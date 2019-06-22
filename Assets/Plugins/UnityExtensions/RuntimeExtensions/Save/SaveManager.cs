using System;
using System.Collections.Generic;

namespace UnityExtensions
{
    /// <summary>
    /// 存档系统任务
    /// </summary>
    public class SaveTask
    {
        /// <summary>
        /// 存档系统任务类型
        /// </summary>
        public enum Type
        {
            Load,
            Save,
            Delete,
        }


        public Type type;                   // 任务类型
        public Save save;                   // 存档对象
        public ISaveTarget target;          // 储存目标
        public byte[] data;                 // 数据内容
        public Exception exception;         // 执行结果 (null 代表成功)


        public bool success => exception == null;
    }


    /// <summary>
    /// 存档管理器
    /// 所有存档相关操作本质都是异步的，但此管理器所有 API 都应当在主线程调用
    /// 针对每一个存档对象，你需要根据需求选择恰当的 SaveTarget
    /// </summary>
    public struct SaveManager
    {
        static Queue<SaveTask> _tasks = new Queue<SaveTask>(4);    // 任务队列

        
        /// <summary>
        /// 任务结束时触发
        /// </summary>
        public static event Action<SaveTask> onTaskFinished;


        /// <summary>
        /// 是否有未完成的任务
        /// 在游戏退出前需检查所有任务是否已经完成
        /// </summary>
        public static bool hasTask
        {
            get { return _tasks.Count > 0; }
        }


        // 新建任务
        static void NewTask(SaveTask.Type type, Save save, ISaveTarget target)
        {
            var task = new SaveTask()
            {
                type = type,
                save = save,
                target = target,
            };

            // 预处理
            switch (type)
            {
                case SaveTask.Type.Save:
                    task.exception = save.ToBytes(out task.data);
                    break;

                default:
                    task.data = null;
                    task.exception = null;
                    break;
            }

            _tasks.Enqueue(task);

            // 第一个任务在创建后立即开始执行
            if (_tasks.Count == 1)
            {
                BeginTask();
            }
        }


        // 开始任务时调用
        static void BeginTask()
        {
            var task = _tasks.Peek();

            switch (task.type)
            {
                case SaveTask.Type.Load:
                    task.target.ReadAsync(task);
                    break;

                case SaveTask.Type.Save:
                    if (task.success)
                    {
                        task.target.WriteAsync(task);
                    }
                    else
                    {
                        EndTask();
                    }
                    break;

                case SaveTask.Type.Delete:
                    task.target.DeleteAsync(task);
                    break;
            }
        }


        // 结束任务时调用
        internal static void EndTask()
        {
            var task = _tasks.Dequeue();

            // 后续处理
            switch (task.type)
            {
                case SaveTask.Type.Load:
                    if (task.success)
                        task.exception = task.save.FromBytes(ref task.data);
                    else
                        task.save.Reset();
                    break;
            }

            onTaskFinished?.Invoke(task);

            if (_tasks.Count > 0)
            {
                BeginTask();
            }
        }


        /// <summary>
        /// 新建保存任务
        /// </summary>
        public static void NewSaveTask(Save save, ISaveTarget target)
        {
            NewTask(SaveTask.Type.Save, save, target);
        }


        /// <summary>
        /// 新建加载任务
        /// </summary>
        public static void NewLoadTask(Save save, ISaveTarget target)
        {
            NewTask(SaveTask.Type.Load, save, target);
        }


        /// <summary>
        /// 新建删除任务
        /// </summary>
        public static void NewDeleteTask(ISaveTarget target)
        {
            NewTask(SaveTask.Type.Delete, null, target);
        }

    } // struct SaveManager

} // namespace UnityExtensions
