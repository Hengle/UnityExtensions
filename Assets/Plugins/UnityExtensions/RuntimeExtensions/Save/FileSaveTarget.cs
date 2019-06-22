using System;
using System.IO;
using System.Threading;

namespace UnityExtensions
{
    /// <summary>
    /// 文件储存目标
    /// </summary>
    public class FileSaveTarget : ISaveTarget
    {
        string _fullDir;
        string _fullPath;

        volatile bool _finished;


        /// <summary>
        /// 创建储存目标
        /// </summary>
        /// <param name="fileName"> 文件名，可使用 "/" 添加父级目录；文件名相对于 Application.persistentDataPath 目录 </param>
        public FileSaveTarget(string fileName)
        {
            _fullPath = string.Format("{0}/{1}", UnityEngine.Application.persistentDataPath, fileName);

            int firstSlash = _fullPath.IndexOf('/');
            int lastSlash = _fullPath.LastIndexOf('/');
            if (firstSlash != lastSlash) _fullDir = _fullPath.Substring(0, lastSlash);
        }


        void MainThreadUpdate()
        {
            if (_finished)
            {
                _finished = false;
                ApplicationKit.update -= MainThreadUpdate;
                SaveManager.EndTask();
            }
        }


        void ISaveTarget.ReadAsync(SaveTask task)
        {
            ApplicationKit.update += MainThreadUpdate;
            ThreadPool.QueueUserWorkItem(BackgroundThreadProc);

            void BackgroundThreadProc(object _)
            {
                try
                {
                    using (var stream = new FileStream(_fullPath, FileMode.Open, FileAccess.Read))
                    {
                        task.data = new byte[stream.Length];
                        stream.Read(task.data, 0, task.data.Length);
                    }
                }
                catch (Exception e)
                {
                    task.exception = e;
                }

                _finished = true;
            }
        }


        void ISaveTarget.WriteAsync(SaveTask task)
        {
            ApplicationKit.update += MainThreadUpdate;
            ThreadPool.QueueUserWorkItem(BackgroundThreadProc);

            void BackgroundThreadProc(object _)
            {
                try
                {
                    if (_fullDir != null) Directory.CreateDirectory(_fullDir);
                    using (var stream = new FileStream(_fullPath, FileMode.Create, FileAccess.Write))
                    {
                        stream.Write(task.data, 0, task.data.Length);
                    }
                }
                catch (Exception e)
                {
                    task.exception = e;
                }

                _finished = true;
            }
        }


        void ISaveTarget.DeleteAsync(SaveTask task)
        {
            ApplicationKit.update += MainThreadUpdate;
            ThreadPool.QueueUserWorkItem(BackgroundThreadProc);

            void BackgroundThreadProc(object _)
            {
                try
                {
                    File.Delete(_fullPath);
                }
                catch (Exception e)
                {
                    task.exception = e;
                }

                _finished = true;
            }
        }

    } // class FileSaveTarget

} // namespace UnityExtensions