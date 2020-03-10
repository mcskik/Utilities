using Copy8.DataLayer.Profile;
using System;
using System.Collections.Specialized;

namespace Copy8
{
    public interface ICopyEngine
    {
        void XCopy(string sourceDirectory, string targetDirectory);
        void XDelete(string sourceDirectory);
        DateTime CompleteTaskStartTime { get; }
        bool Identical { get; }
        Interrupt Interrupt { get; set; }
        UserSetting.CopyRuleEnum CopyRule { get; set; }
        bool MonitoredTypesOnly { get; set; }
        event CopyEngine.BeginCountHandler OnBeginCount;
        event CopyEngine.EndOfCountHandler OnEndOfCount;
        event CopyEngine.UpdateCountHandler OnUpdateCount;
        event CopyEngine.BeginCopyHandler OnBeginCopy;
        event CopyEngine.EndOfCopyHandler OnEndOfCopy;
        event CopyEngine.UpdateCopyHandler OnUpdateCopy;
        event CopyEngine.BeginDeleteHandler OnBeginDelete;
        event CopyEngine.EndOfDeleteHandler OnEndOfDelete;
        event CopyEngine.UpdateDeleteHandler OnUpdateDelete;
    }
}