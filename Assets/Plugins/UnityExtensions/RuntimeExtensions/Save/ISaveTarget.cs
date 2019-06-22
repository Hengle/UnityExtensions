
namespace UnityExtensions
{
    /// <summary>
    /// 储存目标
    /// </summary>
    public interface ISaveTarget
    {
        /// <summary>
        /// 开始异步读取，操作完成后需要在主线程调用 SaveManager.EndTask
        /// </summary>
        void ReadAsync(SaveTask task);


        /// <summary>
        /// 开始异步写入，操作完成后需要在主线程调用 SaveManager.EndTask
        /// </summary>
        void WriteAsync(SaveTask task);


        /// <summary>
        /// 删除储存目标，操作完成后需要在主线程调用 SaveManager.EndTask
        /// </summary>
        void DeleteAsync(SaveTask task);

    } // interface ISaveTarget

} // namespace UnityExtensions