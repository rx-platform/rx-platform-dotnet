using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RxPlatform.Hosting.Interface
{
    internal class PlatformABI
    {
        internal static PlatformABITranslated platformABI = new PlatformABITranslated();

        internal static void InitABI(ref platform_api6 nativeABI)
        {
            if (nativeABI.general.pWriteLog != nint.Zero)
                platformABI.pWriteLog = Marshal.GetDelegateForFunctionPointer<rxWriteLogDelegate>(nativeABI.general.pWriteLog);
            if (nativeABI.general.pRegisterItem != nint.Zero)
                platformABI.pRegisterItem = Marshal.GetDelegateForFunctionPointer<rxRegisterItemDelegate>(nativeABI.general.pRegisterItem);
            if (nativeABI.general.prxRegisterRuntimeItem != nint.Zero)
                platformABI.prxRegisterRuntimeItem = Marshal.GetDelegateForFunctionPointer<rxRegisterRuntimeItemDelegate>(nativeABI.general.prxRegisterRuntimeItem);
            if (nativeABI.general.prxLockRuntimeManager != nint.Zero)
                platformABI.prxLockRuntimeManager = Marshal.GetDelegateForFunctionPointer<rxLockRuntimeManagerDelegate>(nativeABI.general.prxLockRuntimeManager);
            if (nativeABI.general.prxUnlockRuntimeManager != nint.Zero)
                platformABI.prxUnlockRuntimeManager = Marshal.GetDelegateForFunctionPointer<rxUnlockRuntimeManagerDelegate>(nativeABI.general.prxUnlockRuntimeManager);


            if (nativeABI.runtime.prxRegisterSourceRuntime != nint.Zero)
                platformABI.prxRegisterSourceRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterSourceRuntimeDelegate>(nativeABI.runtime.prxRegisterSourceRuntime);
            if (nativeABI.runtime.prxRegisterMapperRuntime != nint.Zero)
                platformABI.prxRegisterMapperRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterMapperRuntimeDelegate>(nativeABI.runtime.prxRegisterMapperRuntime);
            if (nativeABI.runtime.prxRegisterFilterRuntime != nint.Zero)
                platformABI.prxRegisterFilterRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterFilterRuntimeDelegate>(nativeABI.runtime.prxRegisterFilterRuntime);
            if (nativeABI.runtime.prxRegisterStructRuntime != nint.Zero)
                platformABI.prxRegisterStructRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterStructRuntimeDelegate>(nativeABI.runtime.prxRegisterStructRuntime);
            if (nativeABI.runtime.prxRegisterVariableRuntime != nint.Zero)
                platformABI.prxRegisterVariableRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterVariableRuntimeDelegate>(nativeABI.runtime.prxRegisterVariableRuntime);
            if (nativeABI.runtime.prxRegisterEventRuntime != nint.Zero)
                platformABI.prxRegisterEventRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterEventRuntimeDelegate>(nativeABI.runtime.prxRegisterEventRuntime);
            if (nativeABI.runtime.prxRegisterMethodRuntime != nint.Zero)
                platformABI.prxRegisterMethodRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterMethodRuntimeDelegate>(nativeABI.runtime.prxRegisterMethodRuntime);
            if (nativeABI.runtime.prxRegisterProgramRuntime != nint.Zero)
                platformABI.prxRegisterProgramRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterProgramRuntimeDelegate>(nativeABI.runtime.prxRegisterProgramRuntime);
            if (nativeABI.runtime.prxRegisterDisplayRuntime != nint.Zero)
                platformABI.prxRegisterDisplayRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterDisplayRuntimeDelegate>(nativeABI.runtime.prxRegisterDisplayRuntime);
            if (nativeABI.runtime.prxRegisterObjectRuntime != nint.Zero)
                platformABI.prxRegisterObjectRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterObjectRuntimeDelegate>(nativeABI.runtime.prxRegisterObjectRuntime);
            if (nativeABI.runtime.prxRegisterApplicationRuntime != nint.Zero)
                platformABI.prxRegisterApplicationRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterApplicationRuntimeDelegate>(nativeABI.runtime.prxRegisterApplicationRuntime);
            if (nativeABI.runtime.prxRegisterDomainRuntime != nint.Zero)
                platformABI.prxRegisterDomainRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterDomainRuntimeDelegate>(nativeABI.runtime.prxRegisterDomainRuntime);
            if (nativeABI.runtime.prxRegisterPortRuntime != nint.Zero)
                platformABI.prxRegisterPortRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterPortRuntimeDelegate>(nativeABI.runtime.prxRegisterPortRuntime);
            if (nativeABI.runtime.prxRegisterRelationRuntime != nint.Zero)
                platformABI.prxRegisterRelationRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterRelationRuntimeDelegate>(nativeABI.runtime.prxRegisterRelationRuntime);
            if (nativeABI.runtime.prxInitCtxBindItem != nint.Zero)
                platformABI.prxInitCtxBindItem = Marshal.GetDelegateForFunctionPointer<rxInitCtxBindItemDelegate>(nativeABI.runtime.prxInitCtxBindItem);
            if (nativeABI.runtime.prxInitCtxGetCurrentPath != nint.Zero)
                platformABI.prxInitCtxGetCurrentPath = Marshal.GetDelegateForFunctionPointer<rxInitCtxGetCurrentPathDelegate>(nativeABI.runtime.prxInitCtxGetCurrentPath);
            if (nativeABI.runtime.prxInitCtxGetLocalValue != nint.Zero)
                platformABI.prxInitCtxGetLocalValue = Marshal.GetDelegateForFunctionPointer<rxInitCtxGetLocalValueDelegate>(nativeABI.runtime.prxInitCtxGetLocalValue);
            if (nativeABI.runtime.prxInitCtxSetLocalValue != nint.Zero)
                platformABI.prxInitCtxSetLocalValue = Marshal.GetDelegateForFunctionPointer<rxInitCtxSetLocalValueDelegate>(nativeABI.runtime.prxInitCtxSetLocalValue);
            if (nativeABI.runtime.prxInitCtxGetMappingValues != nint.Zero)
                platformABI.prxInitCtxGetMappingValues = Marshal.GetDelegateForFunctionPointer<rxInitCtxGetMappingValuesDelegate>(nativeABI.runtime.prxInitCtxGetMappingValues);
            if (nativeABI.runtime.prxInitCtxGetSourceValues != nint.Zero)
                platformABI.prxInitCtxGetSourceValues = Marshal.GetDelegateForFunctionPointer<rxInitCtxGetSourceValuesDelegate>(nativeABI.runtime.prxInitCtxGetSourceValues);
            if (nativeABI.runtime.prxInitCtxGetItemMeta != nint.Zero)
                platformABI.prxInitCtxGetItemMeta = Marshal.GetDelegateForFunctionPointer<rxInitCtxGetItemMetaDelegate>(nativeABI.runtime.prxInitCtxGetItemMeta);
            if (nativeABI.runtime.prxInitCtxGetDataType != nint.Zero)
                platformABI.prxInitCtxGetDataType = Marshal.GetDelegateForFunctionPointer<rxInitCtxGetDataTypeDelegate>(nativeABI.runtime.prxInitCtxGetDataType);
            if (nativeABI.runtime.prxStartCtxGetCurrentPath != nint.Zero)
                platformABI.prxStartCtxGetCurrentPath = Marshal.GetDelegateForFunctionPointer<rxStartCtxGetCurrentPathDelegate>(nativeABI.runtime.prxStartCtxGetCurrentPath);
            if (nativeABI.runtime.prxStartCtxCreateTimer != nint.Zero)
                platformABI.prxStartCtxCreateTimer = Marshal.GetDelegateForFunctionPointer<rxStartCtxCreateTimerDelegate>(nativeABI.runtime.prxStartCtxCreateTimer);
            if (nativeABI.runtime.prxStartCtxGetLocalValue != nint.Zero)
                platformABI.prxStartCtxGetLocalValue = Marshal.GetDelegateForFunctionPointer<rxStartCtxGetLocalValueDelegate>(nativeABI.runtime.prxStartCtxGetLocalValue);
            if (nativeABI.runtime.prxStartCtxSetLocalValue != nint.Zero)
                platformABI.prxStartCtxSetLocalValue = Marshal.GetDelegateForFunctionPointer<rxStartCtxSetLocalValueDelegate>(nativeABI.runtime.prxStartCtxSetLocalValue);
            if (nativeABI.runtime.prxStartCtxSubscribeRelation != nint.Zero)
                platformABI.prxStartCtxSubscribeRelation = Marshal.GetDelegateForFunctionPointer<rxStartCtxSubscribeRelationDelegate>(nativeABI.runtime.prxStartCtxSubscribeRelation);
            if (nativeABI.runtime.prxCtxGetValue != nint.Zero)
                platformABI.prxCtxGetValue = Marshal.GetDelegateForFunctionPointer<rxCtxGetValueDelegate>(nativeABI.runtime.prxCtxGetValue);
            if (nativeABI.runtime.prxCtxSetValue != nint.Zero)
                platformABI.prxCtxSetValue = Marshal.GetDelegateForFunctionPointer<rxCtxSetValueDelegate>(nativeABI.runtime.prxCtxSetValue);
            if (nativeABI.runtime.prxCtxSetAsyncPending != nint.Zero)
                platformABI.prxCtxSetAsyncPending = Marshal.GetDelegateForFunctionPointer<rxCtxSetAsyncPendingDelegate>(nativeABI.runtime.prxCtxSetAsyncPending);
            if(nativeABI.runtime.prxRegisterDataTypeRuntime!= nint.Zero)
                platformABI.prxRegisterDataTypeRuntime = Marshal.GetDelegateForFunctionPointer<rxRegisterDataTypeRuntimeDelegate>(nativeABI.runtime.prxRegisterDataTypeRuntime);
            if (nativeABI.runtime.prxCtxExecuteConnected!=nint.Zero)
                platformABI.prxCtxExecuteConnected = Marshal.GetDelegateForFunctionPointer<rxCtxExecuteConnectedDelegate>(nativeABI.runtime.prxCtxExecuteConnected);
            if (nativeABI.runtime.prxInitCtxConnectItem != nint.Zero)
                platformABI.prxInitCtxConnectItem = Marshal.GetDelegateForFunctionPointer<rxInitCtxConnectItemDelegate>(nativeABI.runtime.prxInitCtxConnectItem);
            if (nativeABI.runtime.rxGetNewUniqueId != nint.Zero)
                platformABI.rxGetNewUniqueId = Marshal.GetDelegateForFunctionPointer<rxGetNewUniqueIdDelegate>(nativeABI.runtime.rxGetNewUniqueId);
            if (nativeABI.runtime.prxCtxWriteBinded != nint.Zero)
                platformABI.prxCtxWriteBinded = Marshal.GetDelegateForFunctionPointer<rxCtxWriteBindedDelegate>(nativeABI.runtime.prxCtxWriteBinded);
            if (nativeABI.runtime.prxCtxWriteConnected != nint.Zero)
                platformABI.prxCtxWriteConnected = Marshal.GetDelegateForFunctionPointer<rxCtxWriteConnectedDelegate>(nativeABI.runtime.prxCtxWriteConnected);

        }
    }
}
