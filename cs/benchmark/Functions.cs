using System.Runtime.CompilerServices;
using System.Diagnostics;
using FASTER.core;

namespace FASTER.benchmark
{
    /// <summary>
    /// Implementation of FASTER's IFunctions interface for benchmarking.
    /// Provides methods for reading, writing, and modifying data structures in a concurrent environment.
    /// </summary>
    public struct Functions : IFunctions<Key, Value, Input, Output, Empty>
    {
        public void RMWCompletionCallback(ref Key key, ref Input input, ref Output output, Empty ctx, Status status, RecordMetadata recordMetadata)
        {
            Debug.WriteLine($"RMW operation completed with status: {status} for key: {key}");
        }

        public void ReadCompletionCallback(ref Key key, ref Input input, ref Output output, Empty ctx, Status status, RecordMetadata recordMetadata)
        {
            Debug.WriteLine($"Read operation completed with status: {status} for key: {key}");
        }

        public void CheckpointCompletionCallback(int sessionID, string sessionName, CommitPoint commitPoint)
        {
            Debug.WriteLine($"Checkpoint completed for Session {sessionID} ({sessionName ?? "null"}), " +
                            $"persistence until Serial No: {commitPoint.UntilSerialNo}, " +
                            $"commit GUID: {commitPoint.CommitGuid}");
        }

        // Read functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SingleReader(ref Key key, ref Input input, ref Value value, ref Output dst, ref ReadInfo readInfo)
        {
            dst.value = value;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConcurrentReader(ref Key key, ref Input input, ref Value value, ref Output dst, ref ReadInfo readInfo)
        {
            dst.value = value;
            return true;
        }

        // Delete functions
        public bool SingleDeleter(ref Key key, ref Value value, ref DeleteInfo deleteInfo)
        {
            value = default;
            Debug.WriteLine($"Single deletion performed for key: {key}");
            return true;
        }

        public bool ConcurrentDeleter(ref Key key, ref Value value, ref DeleteInfo deleteInfo)
        {
            Debug.WriteLine($"Concurrent deletion performed for key: {key}");
            return true;
        }

        // Upsert functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SingleWriter(ref Key key, ref Input input, ref Value src, ref Value dst, ref Output output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            dst = src;
            Debug.WriteLine($"Single write operation: Key={key}, Value={src}");
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConcurrentWriter(ref Key key, ref Input input, ref Value src, ref Value dst, ref Output output, ref UpsertInfo upsertInfo)
        {
            dst = src;
            Debug.WriteLine($"Concurrent write operation: Key={key}, Value={src}");
            return true;
        }

        // RMW functions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InitialUpdater(ref Key key, ref Input input, ref Value value, ref Output output, ref RMWInfo rmwInfo)
        {
            value.value = input.value;
            Debug.WriteLine($"Initial update for key: {key} with input value: {input.value}");
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InPlaceUpdater(ref Key key, ref Input input, ref Value value, ref Output output, ref RMWInfo rmwInfo)
        {
            value.value += input.value;
            Debug.WriteLine($"In-place update for key: {key} - Updated value: {value.value}");
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CopyUpdater(ref Key key, ref Input input, ref Value oldValue, ref Value newValue, ref Output output, ref RMWInfo rmwInfo)
        {
            newValue.value = input.value + oldValue.value;
            Debug.WriteLine($"Copy update for key: {key} - Old Value: {oldValue.value}, New Value: {newValue.value}");
            return true;
        }

        // Additional post-operation methods to log or modify the result after certain actions
        public void PostCopyUpdater(ref Key key, ref Input input, ref Value oldValue, ref Value newValue, ref Output output, ref RMWInfo rmwInfo)
        {
            Debug.WriteLine($"Post copy update completed for key: {key}");
        }

        public bool NeedInitialUpdate(ref Key key, ref Input input, ref Output output, ref RMWInfo rmwInfo) => true;

        public void PostInitialUpdater(ref Key key, ref Input input, ref Value value, ref Output output, ref RMWInfo rmwInfo)
        {
            Debug.WriteLine($"Post initial update for key: {key} - Value set to: {value.value}");
        }

        public bool NeedCopyUpdate(ref Key key, ref Input input, ref Value oldValue, ref Output output, ref RMWInfo rmwInfo) => true;

        public void PostSingleDeleter(ref Key key, ref DeleteInfo deleteInfo)
        {
            Debug.WriteLine($"Post single delete operation for key: {key}");
        }

        public void PostSingleWriter(ref Key key, ref Input input, ref Value src, ref Value dst, ref Output output, ref UpsertInfo upsertInfo, WriteReason reason)
        {
            Debug.WriteLine($"Post single write for key: {key}");
        }

        // Disposal methods
        public void DisposeSingleWriter(ref Key key, ref Input input, ref Value src, ref Value dst, ref Output output, ref UpsertInfo upsertInfo, WriteReason reason) { }
        public void DisposeCopyUpdater(ref Key key, ref Input input, ref Value oldValue, ref Value newValue, ref Output output, ref RMWInfo rmwInfo) { }
        public void DisposeInitialUpdater(ref Key key, ref Input input, ref Value value, ref Output output, ref RMWInfo rmwInfo) { }
        public void DisposeSingleDeleter(ref Key key, ref Value value, ref DeleteInfo deleteInfo) { }
        public void DisposeDeserializedFromDisk(ref Key key, ref Value value) { }
    }
}
