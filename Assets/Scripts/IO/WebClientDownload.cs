
    using System;
using System.Collections;
using System.IO;
using System.Net;

namespace IO
{
    public abstract class CustomOperation : IEnumerator
    {
        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        abstract public bool isDone { get; }

        abstract public float GetProgress();

        abstract public IEnumerator Cancel();
    }
    public class WebClientDownload : CustomOperation
    {

        private string fileName;
     

        private string m_url;
        private string m_localPath;
        private string m_md5;

        private string temp;

        public const int RETRY_MAX = 3;
        private WebClient m_client;
        private uint m_failedCount = 0;

        public volatile string Error;
        public volatile bool complete = false;

        private float m_downloadProgress;
        private long m_bytesReceived;//当前下载量
        private long m_totalBytesToReceive; //所需下载总量

        public override float GetProgress()
        {
            return m_downloadProgress;
        }


        /// <summary>
        /// 从网络下载到本地
        /// </summary>
        /// <param name="fname">文件名</param>
        /// <param name="url">网络地址</param>
        /// <param name="localPath">本地地址</param>
        /// <param name="md5"></param>
        public WebClientDownload(string fname, string url, string localPath, string md5)
        {
            this.fileName = fname;
            this.m_url = url;
            this.m_localPath = localPath;
            this.m_md5 = md5;
            this.temp = string.Format("{0}.tmp", localPath);
            DoDownload();
        }

        public void Fill(string fname, string url, string localPath, string md5)
        {
            this.fileName = fname;
            this.m_url = url;
            this.m_localPath = localPath;
            this.m_md5 = md5;
            this.temp = string.Format("{0}.tmp", localPath);

            this.m_failedCount = 0;
            this.Error = null;
            this.complete = false;
            this.m_downloadProgress = 0;
            this.m_bytesReceived = 0;
            this.m_totalBytesToReceive = 0;
        }

        public override bool isDone
        {
            get
            {
                if (!complete)
                    return false;

                if (!string.IsNullOrEmpty(Error) && m_failedCount < RETRY_MAX)
                {
                    DoDownload();
                    return false;
                }
                return true;
            }
        }


        private void Fallback()
        {
            if (Error == null)
            {
                File.Move(temp, m_localPath);
            }
            else
            {
               File.Delete(temp);
            }
        }

        private void Dispose()
        {
            if (m_client != null)
            {
                m_client.Dispose();
                m_client = null;
            }
        }

        public override IEnumerator Cancel()
        {
            Dispose();
            yield return null;
        }
        
/// <summary>
/// 下载接口： m_client.DownloadFileAsync(new Uri(finalUrl), temp, this);
/// 下载完成后将文件Move到目标文件上去
/// </summary>
/// <returns></returns>
        public bool DoDownload()
        {
            Error = null;
            complete = false;
            if(File.Exists(m_localPath))
                File.Delete(m_localPath);
            var finalUrl = m_url;
            if (this.m_failedCount > 0)
            {
                finalUrl = string.Format("{0}?time={1}", m_url, DateTime.Now.ToFileTime());
            }
            DirectoryUtil.CreateDirFromFile(temp);

            if (m_client != null)
            {
                m_client.Dispose();
                m_client = null;
            }

            m_client = new WebClient();
            m_client.DownloadFileAsync(new Uri(finalUrl), temp, this);
            m_client.DownloadProgressChanged += (sender, e) =>
            {
                m_downloadProgress = e.ProgressPercentage / 100f;
                m_bytesReceived = e.BytesReceived;
                m_totalBytesToReceive = e.TotalBytesToReceive;
            };

            m_client.DownloadFileCompleted += (sender, e) =>
            {
                WebClientDownload self = (WebClientDownload)e.UserState;

                if (e.Cancelled)
                {
                    self.Error = "WebClient Canceled";
                    goto Finish;
                }

                if (e.Error != null)
                {
                    self.Error = e.Error.Message;
                    goto Finish;
                }

                if (!File.Exists(self.temp))
                {
                    self.Error = "down complete, but tmp file is not exist";
                    goto Finish;
                }

                if (!string.IsNullOrEmpty(self.m_md5)) //适应没有生成md5文件的cdn
                {
                    var tmpMd5 = FileUtil.GetMd5HashStringFromFile(self.temp);
                    if (!tmpMd5.Equals(self.m_md5))
                    {
                        File.Delete(self.temp);
                        self.Error = string.Format("md5 is not match : {0}, {1} != {2}", self.fileName, self.m_md5,
                            tmpMd5);
                        goto Finish;
                    }
                }

                Finish:
                if (self.Error != null)
                {
                    ++self.m_failedCount;
                }

                self.Fallback();
                self.complete = true;
            };
            return true;
        }

        public bool IsSameContent(string url, string localPath, string md5)
        {
            return this.m_url == url && this.m_localPath == localPath && this.m_md5 == md5;
        }
    }
}
