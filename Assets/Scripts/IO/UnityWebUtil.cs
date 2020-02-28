using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace IO
{
    public enum EDownloadContentType
    {
        AssetBundle,
        String,
        Bytes
    }

    public static class UnityWebUtil
    {
        #region //UnityWeb Extension

        //首先拿到文件的全部长度
        public static long GetDownloadFileLength(this UnityWebRequest webRequest)
        {
            return long.Parse(webRequest.GetResponseHeader("Content-Length"));
        }

        public static UnityWebRequest SendRequestWithSpecifyLength(string url,long currFileLength,long downloadFileLength)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Range", "bytes=" + currFileLength + "-" + downloadFileLength);
            return request;
        }

        #endregion
      
        #region //Download ==>Get

        /// <summary>
        ///     Download AssetBundle from Get Method
        /// </summary>
        public static IEnumerator DownloadBundle(string url, uint crc, Action<string, AssetBundle> callback)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "GET");
            //设置Handler
            webRequest.downloadHandler = new DownloadHandlerAssetBundle(url, crc);
            //设置过期时间
            webRequest.timeout = 5;
            yield return webRequest.Send();
            AssetBundle content = (webRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
            if (webRequest.isError)
            {
            }

            callback(webRequest.error, content);
        }

        /// <summary>
        ///     Download Text from Get Method
        /// </summary>
        public static IEnumerator DownloadText(string url, uint crc, Action<string, string> callback)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "GET");
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.Send();
            var text = (webRequest.downloadHandler as DownloadHandlerBuffer).text;
            if (webRequest.isError)
            {
            }

            callback(webRequest.error, text);
        }

        /// <summary>
        ///     Download Bytes from Get Method
        /// </summary>
        public static IEnumerator DownloadBytes(string url, uint crc, Action<string, byte[]> callback)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "GET");
            // 静态方法构建
            webRequest = UnityWebRequest.Get(url);
            //Head创建
            webRequest =
                            UnityWebRequest.Head("http://www.chinar.xin/chinarweb/WebRequest/Get/00-效果.mp4");
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.Send();
            // 设置取消方法
            webRequest.Abort();

            var data = (webRequest.downloadHandler as DownloadHandlerBuffer).data;
            //查看返回结果
            if (webRequest.isError)
            {
            }
            else
            {
                long totalLength = long.Parse(webRequest.GetResponseHeader("Content-Length"));
                //....Do Someth
            }

            callback(webRequest.error, data);
        }

        #endregion

        #region //Download==>post,put

        public static IEnumerator PostForm(WWWForm wwwForm, string url)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(url, wwwForm);
            yield return webRequest.Send();
            if (webRequest.isError)
            {
            }
        }

        public static IEnumerator PostData(Byte[] bytes, string url)
        {
            UnityWebRequest webRequest = UnityWebRequest.Put(url, bytes);
            yield return webRequest.Send();

            if (webRequest.isError)
            {
            }
        }

        #endregion

        #region //Continue DownloadFile,Continue DownloadFile better

        /// <summary>
        ///     用于边下载边展示的情况,将下载的文件流放在内存中，最后再一起写入
        /// </summary>
        public static IEnumerator DownloadFile(string url)
        {
            var request = UnityWebRequest.Get(url);
            //此处不适用yield return request.Send();
            request.Send();
            if (request.isError)
            {
                yield break;
            }

            while (!request.isDone)
            {
                float progress = request.downloadProgress;
            }

            byte[] results = request.downloadHandler.data;
            // 注意真机上要用Application.persistentDataPath
            FileUtil.CreateFile(Application.streamingAssetsPath + "/MP4/test.mp4", results,
                request.downloadHandler.data.Length);
            AssetDatabase.Refresh(); //刷新一下
        }
        /// <summary>
        /// 断点续传功能实现：
        /// 1.Head发送UnityWebRequest获取文件总长度
        /// 2.创建文件流，获取当前写入文件总长度并且移动到文件尾部 webRequest.GetResponseHeader("Content-Length"));
        /// 3.发送UnityWebRequest请求文件缺失的那一部分数据 SetRequestHeader("Range", "bytes=" + currFileLength + "-" + downloadFileLength);]
        /// 4.While (!request.isDone)不停写入文件，直到文件写满
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IEnumerator StopContinueDownloadFile(string url)
        {
            var progress = 0f;
            //Head创建:只请求头文件信息
            var request = UnityWebRequest.Head(url);
            yield return request.Send();
            if (request.isError) //如果出错
            {
                yield break;
            }
            //获取头文件信息：File长度
            var downloadFileLength = request.GetDownloadFileLength();
            //把它清理掉
            request.Dispose();
            FileUtil.CreateFileByCallback(url, fs =>
            {
                long currFileLength = fs.Length;
                //获取要下载的长度足够大
                if (currFileLength >= downloadFileLength)
                {
                    progress = 1f;
                    fs.Close();
                    fs.Dispose();
                    return;
                }

                //到达文件数据预定位置
                fs.Seek(currFileLength, SeekOrigin.Begin);
                //请求一点范围内的数据
                request = SendRequestWithSpecifyLength(url,currFileLength,downloadFileLength);
                //此处不适用yield return request.Send();
                request.Send();

                var index = 0;
                while (!request.isDone)
                {
                    var buff = request.downloadHandler.data;
                    if (buff != null)
                    {
                        var length = buff.Length - index;
                        fs.Write(buff, index, length);
                        index      += length;
                        currFileLength += length;

                        if (currFileLength >= downloadFileLength)
                        {
                            progress = 1f;
                        }
                        else
                        {
                            progress = currFileLength / (float) downloadFileLength;
                        }
                    }
                }
            });
            if (progress >= 1f)
            {
                //...返回成功回调
            }
        }

        #endregion
    }
}