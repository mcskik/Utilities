using System;

namespace Lookout.Models
{
    public interface ILookoutMover
    {
        DateTime CompleteTaskStartTime { get; }
        Interrupt Interrupt { get; set; }
        event LookoutMover.BeginCountHandler OnBeginCount;
        event LookoutMover.UpdateCountHandler OnUpdateCount;
        event LookoutMover.EndOfCountHandler OnEndOfCount;
        event LookoutMover.BeginScanHandler OnBeginScan;
        event LookoutMover.UpdateScanHandler OnUpdateScan;
        event LookoutMover.EndOfScanHandler OnEndOfScan;
        event LookoutMover.BeginMoveHandler OnBeginMove;
        event LookoutMover.UpdateMoveHandler OnUpdateMove;
        event LookoutMover.EndOfMoveHandler OnEndOfMove;
        bool Exists(string outlookFolder);
        void MoveMail();
    }
}