#define DEBUG
using System;
using System.Diagnostics;
using System.Text;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.TargetSettings;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class PouDefinitions
	{
		public static readonly string ResetOutputsPouName = "IoConfigResetOutputs_{0}";

		public static readonly string ResetOutputs_Interface = "\r\nFUNCTION IoConfigResetOutputs_{0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpApplicationInfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n    udiState : UDINT;\r\n    pConnectorMapList : POINTER TO IoConfigConnectorMap;\r\n\tnCount : DINT;\r\n\ti: DINT;\r\n\tj: DINT;\r\n    k: DINT;\r\n\tw: DINT;\r\n\tpbyIecAddress : POINTER TO BYTE;\r\n\tpOutputs : POINTER TO BYTE;\r\n\tpConMap : POINTER TO IoConfigConnectorMap;\r\n\tpChannelMap : POINTER TO IoConfigChannelMap;\r\n\tpParam : POINTER TO IoConfigParameter;\r\n\tpConfigTaskInfo : POINTER TO IoConfigTaskMap;\r\n\tpbyDest : POINTER TO BYTE;\r\n\twDestIndex : WORD;\t\r\n\twSize : WORD;\r\n\tbyDestMask : BYTE;\t\r\n    bySrcMask : BYTE;\t\r\nEND_VAR\r\nVAR_STAT\r\n\tudiOldState: UDINT := 2;\r\nEND_VAR\r\n";

		public static readonly string ResetOutputs_InterfaceException = "\r\nFUNCTION IoConfigResetOutputs_{0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpApplicationInfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n    udiState : UDINT;\r\n    udOpState : UDINT;\r\n    pConnectorMapList : POINTER TO IoConfigConnectorMap;\r\n\tnCount : DINT;\r\n\ti: DINT;\r\n\tj: DINT;\r\n    k: DINT;\r\n\tw: DINT;\r\n\tpbyIecAddress : POINTER TO BYTE;\r\n\tpOutputs : POINTER TO BYTE;\r\n\tpConMap : POINTER TO IoConfigConnectorMap;\r\n\tpChannelMap : POINTER TO IoConfigChannelMap;\r\n\tpParam : POINTER TO IoConfigParameter;\r\n\tpConfigTaskInfo : POINTER TO IoConfigTaskMap;\r\n\tpbyDest : POINTER TO BYTE;\r\n\twDestIndex : WORD;\t\r\n\twSize : WORD;\r\n\tbyDestMask : BYTE;\t\r\n    bySrcMask : BYTE;\t\r\nEND_VAR\r\nVAR_STAT\r\n\tudiOldState: UDINT := 2;\r\nEND_VAR\r\n";

		public static readonly string ResetOutputs_InterfaceException64Bit = "\r\nFUNCTION IoConfigResetOutputs_{0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpApplicationInfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n    udiState : UDINT;\r\n    udOpState : UDINT;\r\n    pConnectorMapList : POINTER TO IoConfigConnectorMap;\r\n\tnCount : __XWORD;\r\n\ti: __XWORD;\r\n\tj: DINT;\r\n    k: DINT;\r\n\tw: __XWORD;\r\n\tpbyIecAddress : POINTER TO BYTE;\r\n\tpOutputs : POINTER TO BYTE;\r\n\tpConMap : POINTER TO IoConfigConnectorMap;\r\n\tpChannelMap : POINTER TO IoConfigChannelMap;\r\n\tpParam : POINTER TO IoConfigParameter;\r\n\tpConfigTaskInfo : POINTER TO IoConfigTaskMap;\r\n\tpbyDest : POINTER TO BYTE;\r\n\twDestIndex : WORD;\t\r\n\twSize : WORD;\r\n\tbyDestMask : BYTE;\t\r\n    bySrcMask : BYTE;\t\r\nEND_VAR\r\nVAR_STAT\r\n\tudiOldState: UDINT := 2;\r\nEND_VAR\r\n";

		public static readonly string ResetOutputsDefault_Old = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nIF  udiState = 2 and udiOldState <> 2 THEN\r\n";

		public static readonly string ResetOutputsDefault_New = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nIF  udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0 OR (udOpState AND 16#10000) <>  0 THEN\r\n";

		public static readonly string ResetOutputsDefault_UpdateIosInStop = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\n    IF (udOpState AND 16#400) <> 0 THEN\r\n        // Reset in progress\r\n        udiOldState := 2; // do not execute reset outputs to default\r\n    END_IF\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 2;\r\n    udOpState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\n\r\nIF  udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0 THEN\r\n";

		public static readonly string ResetOutputsDefault_Prg = "    {0}\r\n    pConfigTaskInfo := ADR(iotaskmap[{1}]);\r\n    IF pConfigTaskInfo <> 0 THEN\r\n        w := 0;\r\n        WHILE w < pConfigTaskInfo^.wNumOfConnectorMap DO\r\n\t        pConnectorMapList := ADR(pConfigTaskInfo^.pConnectorMapList[w]);\r\n            IF pConnectorMapList <> 0 THEN\r\n\t            nCount := DWORD_TO_DINT(pConnectorMapList^.dwIoMgrSpecific);\r\n   \r\n\t            FOR i:=0 TO nCount - 1 DO\r\n\t\t            pConMap := ADR(pConnectorMapList[i]);\r\n\t\t            FOR j:= 0 TO DWORD_TO_DINT(pConMap^.dwNumOfChannels) - 1 DO\r\n\t\t\t            pChannelMap := ADR(pConMap^.pChannelMapList[j]);\r\n\t\t\t            pParam := pChannelMap^.pParameter;\r\n\t\t\t            IF (pParam^.dwFlags AND 16#2) <> 0 THEN\r\n\t\t\t\t            pOutputs := pParam^.dwValue;\t\t\t\r\n\t\t\t            ELSE\r\n\t\t\t\t            pOutputs := ADR(pParam^.dwValue);\r\n\t\t\t            END_IF\r\n                        pOutputs := pOutputs + pChannelMap^.wParameterBitOffset / 8;\r\n            \t\t\t\t\r\n\t\t\t            pbyIecAddress := pChannelMap^.pbyIecAddress;\r\n\t\t\t            wDestIndex := pChannelMap^.wIecAddressBitOffset / 8; \r\n\t\t\t            IF (pChannelMap^.wSize = 1) THEN\r\n                            bySrcMask := 1;\r\n\t\t\t\t            bySrcMask := SHL(bySrcMask, pChannelMap^.wParameterBitOffset MOD 8);\r\n\t\t\t\t            byDestMask := 1;\r\n\t\t\t\t            byDestMask := SHL(byDestMask, pChannelMap^.wIecAddressBitOffset MOD 8);\r\n\t\t\t\t            IF (pOutputs^ AND bySrcMask) <> 0 THEN\r\n\t\t\t\t\t            pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] OR byDestMask);\t\t\t\t\r\n\t\t\t\t            ELSE\r\n\t\t\t\t\t            pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] AND NOT byDestMask);\t\t\t\t\r\n\t\t\t\t            END_IF \t\t\t\r\n\t\t\t            ELSE\r\n\t\t\t\t            wSize := (pChannelMap^.wSize / 8);\r\n\t\t\t\t            IF (wSize = 0) THEN\r\n\t\t\t\t\t            CONTINUE;\r\n\t\t\t\t            END_IF\r\n\t\t\t\t            pbyDest := ADR(pbyIecAddress[wDestIndex]);\r\n\t\t\t\t            FOR k:=0 TO wSize - 1 DO\r\n\t\t\t\t\t            pbyDest[k] := pOutputs[k];\r\n\t\t\t\t            END_FOR\r\n\t\t\t            END_IF\r\n\t\t            END_FOR\r\n\t            END_FOR\r\n    \r\n            END_IF\r\n            IF nCount = 0 THEN\r\n                w := w + 1;\r\n            ELSE\r\n                w := w + nCount;\r\n            END_IF\r\n        END_WHILE\r\n        IoMgrWriteOutputs(pConfigTaskInfo);\r\n        {2}\r\n        {3}\r\n    END_IF\r\nEND_IF\r\n\r\nudiOldState := udiState ;\r\n\r\n";

		public static readonly string ResetOutputsDefault_Prg64Bit = "    {0}\r\n    pConfigTaskInfo := ADR(iotaskmap[{1}]);\r\n    IF pConfigTaskInfo <> 0 THEN\r\n        w := 0;\r\n        WHILE w < pConfigTaskInfo^.wNumOfConnectorMap DO\r\n\t        pConnectorMapList := ADR(pConfigTaskInfo^.pConnectorMapList[w]);\r\n            IF pConnectorMapList <> 0 THEN\r\n\t            nCount := pConnectorMapList^.dwIoMgrSpecific;\r\n\t            FOR i:=0 TO nCount - 1 DO\r\n\t\t            pConMap := ADR(pConnectorMapList[i]);\r\n\t\t            FOR j:= 0 TO DWORD_TO_DINT(pConMap^.dwNumOfChannels) - 1 DO\r\n\t\t\t            pChannelMap := ADR(pConMap^.pChannelMapList[j]);\r\n\t\t\t            pParam := pChannelMap^.pParameter;\r\n\t\t\t            IF (pParam^.dwFlags AND 16#2) <> 0 THEN\r\n\t\t\t\t            pOutputs := pParam^.dwValue;\t\t\t\r\n\t\t\t            ELSE\r\n\t\t\t\t            pOutputs := ADR(pParam^.dwValue);\r\n\t\t\t            END_IF\r\n                        pOutputs := pOutputs + pChannelMap^.wParameterBitOffset / 8;\r\n            \t\t\t\t\r\n\t\t\t            pbyIecAddress := pChannelMap^.pbyIecAddress;\r\n\t\t\t            wDestIndex := pChannelMap^.wIecAddressBitOffset / 8; \r\n\t\t\t            IF (pChannelMap^.wSize = 1) THEN\r\n                            bySrcMask := 1;\r\n\t\t\t\t            bySrcMask := SHL(bySrcMask, pChannelMap^.wParameterBitOffset MOD 8);\r\n\t\t\t\t            byDestMask := 1;\r\n\t\t\t\t            byDestMask := SHL(byDestMask, pChannelMap^.wIecAddressBitOffset MOD 8);\r\n\t\t\t\t            IF (pOutputs^ AND bySrcMask) <> 0 THEN\r\n\t\t\t\t\t            pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] OR byDestMask);\t\t\t\t\r\n\t\t\t\t            ELSE\r\n\t\t\t\t\t            pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] AND NOT byDestMask);\t\t\t\t\r\n\t\t\t\t            END_IF \t\t\t\r\n\t\t\t            ELSE\r\n\t\t\t\t            wSize := (pChannelMap^.wSize / 8);\r\n\t\t\t\t            IF (wSize = 0) THEN\r\n\t\t\t\t\t            CONTINUE;\r\n\t\t\t\t            END_IF\r\n\t\t\t\t            pbyDest := ADR(pbyIecAddress[wDestIndex]);\r\n\t\t\t\t            FOR k:=0 TO wSize - 1 DO\r\n\t\t\t\t\t            pbyDest[k] := pOutputs[k];\r\n\t\t\t\t            END_FOR\r\n\t\t\t            END_IF\r\n\t\t            END_FOR\r\n\t            END_FOR\r\n    \r\n            END_IF\r\n            IF nCount = 0 THEN\r\n                w := w + 1;\r\n            ELSE\r\n                w := w + nCount;\r\n            END_IF\r\n        END_WHILE\r\n        IoMgrWriteOutputs(pConfigTaskInfo);\r\n        {2}\r\n        {3}\r\n    END_IF\r\nEND_IF\r\n\r\nudiOldState := udiState ;\r\n\r\n";

		public static readonly string ResetOutputsDefault_Prg64BitNoGlitch = "  pConfigTaskInfo := ADR(iotaskmap[{0}]);\r\n        IF pConfigTaskInfo <> 0 THEN\r\n        w := 0;\r\n        WHILE w < pConfigTaskInfo^.wNumOfConnectorMap DO\r\n\t        pConnectorMapList := ADR(pConfigTaskInfo^.pConnectorMapList[w]);\r\n            IF pConnectorMapList <> 0 THEN\r\n\t            nCount := pConnectorMapList^.dwIoMgrSpecific;\r\n\t            FOR i:=0 TO nCount - 1 DO\r\n\t\t            pConMap := ADR(pConnectorMapList[i]);\r\n\t\t            FOR j:= 0 TO DWORD_TO_DINT(pConMap^.dwNumOfChannels) - 1 DO\r\n\t\t\t            pChannelMap := ADR(pConMap^.pChannelMapList[j]);\r\n\t\t\t            pParam := pChannelMap^.pParameter;\r\n\t\t\t            IF (pParam^.dwFlags AND 16#2) <> 0 THEN\r\n\t\t\t\t            pOutputs := pParam^.dwValue;\t\t\t\r\n\t\t\t            ELSE\r\n\t\t\t\t            pOutputs := ADR(pParam^.dwValue);\r\n\t\t\t            END_IF\r\n                        pOutputs := pOutputs + pChannelMap^.wParameterBitOffset / 8;\r\n            \t\t\t\t\r\n\t\t\t            pbyIecAddress := pChannelMap^.pbyIecAddress;\r\n\t\t\t            wDestIndex := pChannelMap^.wIecAddressBitOffset / 8; \r\n\t\t\t            IF (pChannelMap^.wSize = 1) THEN\r\n                            bySrcMask := 1;\r\n\t\t\t\t            bySrcMask := SHL(bySrcMask, pChannelMap^.wParameterBitOffset MOD 8);\r\n\t\t\t\t            byDestMask := 1;\r\n\t\t\t\t            byDestMask := SHL(byDestMask, pChannelMap^.wIecAddressBitOffset MOD 8);\r\n\t\t\t\t            IF (pOutputs^ AND bySrcMask) <> 0 THEN\r\n\t\t\t\t\t            pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] OR byDestMask);\t\t\t\t\r\n\t\t\t\t            ELSE\r\n\t\t\t\t\t            pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] AND NOT byDestMask);\t\t\t\t\r\n\t\t\t\t            END_IF \t\t\t\r\n\t\t\t            ELSE\r\n\t\t\t\t            wSize := (pChannelMap^.wSize / 8);\r\n\t\t\t\t            IF (wSize = 0) THEN\r\n\t\t\t\t\t            CONTINUE;\r\n\t\t\t\t            END_IF\r\n\t\t\t\t            pbyDest := ADR(pbyIecAddress[wDestIndex]);\r\n\t\t\t\t            FOR k:=0 TO wSize - 1 DO\r\n\t\t\t\t\t            pbyDest[k] := pOutputs[k];\r\n\t\t\t\t            END_FOR\r\n\t\t\t            END_IF\r\n\t\t            END_FOR\r\n\t            END_FOR\r\n    \r\n            END_IF\r\n            IF nCount = 0 THEN\r\n                w := w + 1;\r\n            ELSE\r\n                w := w + nCount;\r\n            END_IF\r\n        END_WHILE\r\n    END_IF\r\nEND_IF\r\n\r\nudiOldState := udiState ;\r\n\r\n";

		public static readonly string ResetOutputsProgram_Body = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nIF  udiState = 2 and udiOldState <> 2 THEN\r\n    {0}();\r\n    {1}\r\n    pConfigTaskInfo := ADR(iotaskmap[{2}]);\r\n    IF pConfigTaskInfo <> 0 THEN\r\n        IoMgrWriteOutputs(pConfigTaskInfo);\r\n    END_IF\r\n    {3}\r\n    {4}\r\nEND_IF\r\nudiOldState := udiState;\r\n\r\n";

		public static readonly string ResetOutputsProgram_BodyException = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nIF  udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0 THEN\r\n    {0}();\r\n    {1}\r\n    pConfigTaskInfo := ADR(iotaskmap[{2}]);\r\n    IF pConfigTaskInfo <> 0 THEN\r\n        IoMgrWriteOutputs(pConfigTaskInfo);\r\n    END_IF\r\n    {3}\r\n    {4}\r\nEND_IF\r\nudiOldState := udiState;\r\n\r\n";

		public static readonly string ResetOutputsProgram_BodyUpdateIosInStop = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\n    IF (udOpState AND 16#400) <> 0 THEN\r\n        // Reset in progress\r\n        udiOldState := 2; // do not execute \r\n\tEND_IF\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nIF  udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0 THEN\r\n    {0}();\r\n    {1}\r\n    pConfigTaskInfo := ADR(iotaskmap[{2}]);\r\n    IF pConfigTaskInfo <> 0 THEN\r\n        IoMgrWriteOutputs(pConfigTaskInfo);\r\n    END_IF\r\n    {3}\r\n    {4}\r\nEND_IF\r\nudiOldState := udiState;\r\n\r\n";

		public static readonly string ResetOutputsProgram_BodyUpdateNoGlitch = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\n    IF (udOpState AND 16#400) <> 0 THEN\r\n        // Reset in progress\r\n        udiOldState := 2; // do not execute \r\n\tEND_IF\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nIF  udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0 THEN\r\n    {0}();\r\nEND_IF\r\nudiOldState := udiState;\r\n\r\n";

		public static readonly string ResetOutputsProgram_BodyUpdateNoGlitchOnlyOnce = "\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\n    IF (udOpState AND 16#400) <> 0 THEN\r\n        // Reset in progress\r\n        udiOldState := 2; // do not execute \r\n\tEND_IF\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nIF  (udiState = 2  OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0) AND udiOldState <> 2 THEN\r\n    {0}();\r\nEND_IF\r\nudiOldState := udiState;\r\n\r\n";

		public static readonly string FindParameter_PouName = "IoConfigFindParameter";

		public static readonly string FindParameter_Interface = "FUNCTION IoConfigFindParameter : POINTER TO IoConfigParameter\r\nVAR_INPUT\r\n\tpConnector : POINTER TO IoConfigConnector;\r\n\tdwParameterId : DWORD;\r\nEND_VAR\r\nVAR\r\n\tnIndex : UDINT;\r\nEND_VAR\r\n";

		public static readonly string FindParameter_Body = "FOR nIndex := 0 TO pConnector^.dwNumOfParameters-1 DO\r\n\tIF pConnector^.pParameterList[nIndex].dwParameterId = dwParameterId THEN\r\n\t\tIoConfigFindParameter := ADR(pConnector^.pParameterList[nIndex]);\r\n\t\tRETURN;\r\n\tEND_IF\r\nEND_FOR\r\n\r\nIoConfigFindParameter := 0;\r\n";

		public static readonly string BeforeTask_PouName = "IoConfigBeforeTask_{0}";

		public static readonly string BeforeTask_Interface = "\r\nFUNCTION IoConfigBeforeTask_{0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpapplicationinfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n\ti : DWORD;\r\n\tbAppHalted : BOOL;\r\nEND_VAR\r\n";

		public static readonly string BeforeTask_Interface_No_Cycle_Control = "\r\nFUNCTION IoConfigBeforeTask_{0} : BOOL\r\nVAR\r\n\ti : DWORD;\r\n\tbAppHalted : BOOL;\r\nEND_VAR\r\n";

		public static readonly string BeforeTask_Body = "\r\n(* IoConfigLateInit call *)\r\n{2}\r\n\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nbAppHalted := (pApplicationInfo^.udState = 2) OR (pApplicationInfo^.udState = 3);\r\n\r\nIF NOT bAppHalted THEN\r\n\t(* Watchdog triggers *)\r\n\t{0}\r\n\r\n\tIoMgrReadInputs(ADR(iotaskmap[{1}]));\r\n    {3}\r\nEND_IF\r\n\r\n";

		public static readonly string BeforeTask_BodyNoBP = "\r\n(* IoConfigLateInit call *)\r\n{2}\r\n\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nbAppHalted := (pApplicationInfo^.udState = 2);\r\n\r\nIF NOT bAppHalted THEN\r\n\t(* Watchdog triggers *)\r\n\t{0}\r\n\r\n\tIoMgrReadInputs(ADR(iotaskmap[{1}]));\r\n    {3}\r\nEND_IF\r\n\r\n";

		public static readonly string BeforeTask_BodyUpdateInStop = "\r\n(* IoConfigLateInit call *)\r\n{2}\r\n\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\n\r\n\r\n(* Watchdog triggers *)\r\n{0}\r\n\r\nIoMgrReadInputs(ADR(iotaskmap[{1}]));\r\n{3}\r\n\r\n\r\n";

		public static readonly string AfterTask_PouName = "IoConfigAfterTask_{0}";

		public static readonly string AfterTask_Interface = "\r\nFUNCTION IoConfigAfterTask_{0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpapplicationinfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n\tbAppHalted : BOOL;\r\nEND_VAR\r\n";

		public static readonly string AfterTask_Interface_No_Cycle_Control = "\r\nFUNCTION IoConfigAfterTask_{0} : BOOL\r\n\r\nVAR\r\n\tbAppHalted : BOOL;\r\nEND_VAR\r\n";

		public static readonly string AfterTask_InterfaceNoGlitch = "\r\nFUNCTION IoConfigAfterTask_{0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpapplicationinfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n\tbAppHalted : BOOL;\r\n    udiState : UDINT;\r\n    udOpState : UDINT;\r\nEND_VAR\r\nVAR_STAT\r\n\tudiOldState: UDINT := 2;\r\nEND_VAR\r\n";

		public static readonly string AfterTask_InterfaceNoGlitch_35110 = "\r\nFUNCTION IoConfigAfterTask_{0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpapplicationinfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n\tbAppHalted : BOOL;\r\n    udiState : UDINT;\r\n    udOpState : UDINT;\r\nEND_VAR\r\nVAR_STAT\r\n\tudiOldState: UDINT := 2;\r\n    udOldOpState : UDINT := 0;\r\nEND_VAR\r\n";

		public static readonly string AfterTask_Body = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nbAppHalted := (pApplicationInfo^.udState = 2) OR (pApplicationInfo^.udState = 3);\r\n\r\nIF NOT bAppHalted THEN\r\n    {2}\r\n\tIoMgrWriteOutputs(ADR(iotaskmap[{0}]));\r\n\r\n\t(* Buscycle calls *)\r\n\t{1}\r\nEND_IF\r\n";

		public static readonly string AfterTask_BodyInStop = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\n{2}\r\nIoMgrWriteOutputs(ADR(iotaskmap[{0}]));\r\n\r\n\r\n(* Buscycle calls *)\r\n{1}\r\n";

		public static readonly string AfterTask_BodyNoGlitch = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\nbAppHalted := (pApplicationInfo^.udState = 2) OR (pApplicationInfo^.udState = 3);\r\nIF  NOT bAppHalted OR udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0 THEN\r\n    {2}\r\n\tIoMgrWriteOutputs(ADR(iotaskmap[{0}]));\r\n\r\n\t(* Buscycle calls *)\r\n\t{1}\r\nEND_IF\r\nudiOldState := udiState;\r\n";

		public static readonly string AfterTask_BodyNoGlitchNoBP = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\nbAppHalted := (pApplicationInfo^.udState = 2);\r\nIF  NOT bAppHalted OR udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0 THEN\r\n    {2}\r\n\tIoMgrWriteOutputs(ADR(iotaskmap[{0}]));\r\n\r\n\t(* Buscycle calls *)\r\n\t{1}\r\nEND_IF\r\nudiOldState := udiState;\r\n";

		public static readonly string AfterTask_BodyNoGlitchNoBP_35110 = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\n    bAppHalted := (pApplicationInfo^.udState = 2);\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n\r\nIF  NOT bAppHalted OR udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0 and (udOldOpState and 16#20) = 0 OR (udOpState AND 16#10000) <>  0 THEN\r\n    {2}\r\n\tIoMgrWriteOutputs(ADR(iotaskmap[{0}]));\r\n\r\n\t(* Buscycle calls *)\r\n\t{1}\r\nEND_IF\r\nudiOldState := udiState;\r\nudOldOpState := udOpState;\r\n";

		public static readonly string AfterTask_BodyNoGlitchNoBPKeepCurrentValue_35110 = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState := pApplicationInfo^.udOpState;\r\n    bAppHalted := (pApplicationInfo^.udState = 2);\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n\r\nIF  NOT bAppHalted OR udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0 and (udOldOpState and 16#20) = 0 OR (udOpState AND 16#10000) <>  0 THEN\r\n    IF (udOpState AND 16#420) = 0 THEN (* do not write outputs on reset or exception because Keep current values is enabled *) \r\n        {2}\r\n\t    IoMgrWriteOutputs(ADR(iotaskmap[{0}]));\r\n\r\n\t    (* Buscycle calls *)\r\n\t    {1}\r\n    END_IF\r\nEND_IF\r\nudiOldState := udiState;\r\nudOldOpState := udOpState;\r\n";

		private static readonly string GlobalInitPou_Name = "IoGlobalInit__Pou";

		private static readonly string GlobalInitPou_Interface = "\r\n{implicit}\r\n{attribute 'call_after_global_init_slot':='1000'}\r\nPROGRAM IoGlobalInit__Pou\r\nVAR\r\n\t__i : DWORD;\r\nEND_VAR\r\n";

		private static readonly string GlobalInitPou_Interface_NoUpdateMapping = "\r\n{implicit}\r\n{attribute 'call_after_global_init_slot':='1000'}\r\nPROGRAM IoGlobalInit__Pou\r\nVAR\r\n\t__i : DWORD;\r\nEND_VAR\r\nVAR_INPUT\r\n   __bNoIoMgrUpdateMapping : BOOL := FALSE;\r\nEND_VAR\r\n";

		private static readonly string GlobalInitPou_Body_NoUpdateMapping = "\r\n{{implicit}}\r\nIF NOT bGlobalInitDone THEN \r\n\r\n\t(* Initialization of FB's *)\r\n\t{3}\r\n\r\n\tIoMgrUpdateConfiguration(pConnectorList := {0}, nCount := {1});\r\n\t\r\n\t(* Initialization after UpdateConfiguration *)\r\n\t{4}\r\n\r\n\tbGlobalInitDone := TRUE;\r\nEND_IF\r\n\r\n(* This shall be called during online change and global init *)\r\n\r\n{{IF NOT defined(IoConfigLateInit)}}\r\n(* Only call IoMgrUpdateMapping if the mapping really changed! (__bNoIoMgrUpdateMapping is set by compiler) *)\r\nIF NOT(__bNoIoMgrUpdateMapping) THEN\r\n\tIoMgrUpdateMapping(ADR(iotaskmap[0]), {2});\r\nEND_IF\r\n{{ELSE}}\r\nIoMgrUpdateMapping(ADR(iotaskmap[0]), {2});\r\n{{END_IF}}\r\n{5}\r\n";

		private static readonly string GlobalInitPou_Body = "\r\n{{implicit}}\r\nIF NOT bGlobalInitDone THEN \r\n\r\n\t(* Initialization of FB's *)\r\n\t{3}\r\n\r\n\tIoMgrUpdateConfiguration(pConnectorList := {0}, nCount := {1});\r\n\t\r\n\t(* Initialization after UpdateConfiguration *)\r\n\t{4}\r\n\r\n\tbGlobalInitDone := TRUE;\r\nEND_IF\r\n\r\n(* This shall be called during online change and global init *)\r\nIoMgrUpdateMapping(ADR(iotaskmap[0]), {2});\r\n{5}\r\n";

		private static readonly string GlobalInitPou_Body2_NoUpdateMapping = "\r\n{{implicit}}\r\nIF NOT bGlobalInitDone THEN \r\n\r\n\t(* Initialization of FB's *)\r\n\t{3}\r\n\r\n\tIoMgrUpdateConfiguration2(pConnectorList := {0}, nCount := {1}, pszConfigApplication := {5});\r\n\t\r\n\t(* Initialization after UpdateConfiguration *)\r\n\t{4}\r\n\r\n\tbGlobalInitDone := TRUE;\r\nEND_IF\r\n\r\n(* This shall be called during online change and global init *)\r\n(* Only call IoMgrUpdateMapping if the mapping really changed! (__bNoIoMgrUpdateMapping is set by compiler) *)\r\nIF NOT(__bNoIoMgrUpdateMapping) THEN\r\n\tIoMgrUpdateMapping2(ADR(iotaskmap[0]), {2}, pszConfigApplication := {5});\r\nEND_IF\r\n{6}\r\n";

		private static readonly string GlobalInitPou_Body2 = "\r\n{{implicit}}\r\nIF NOT bGlobalInitDone THEN \r\n\r\n\t(* Initialization of FB's *)\r\n\t{3}\r\n\r\n\tIoMgrUpdateConfiguration2(pConnectorList := {0}, nCount := {1}, pszConfigApplication := {5});\r\n\t\r\n\t(* Initialization after UpdateConfiguration *)\r\n\t{4}\r\n\r\n\tbGlobalInitDone := TRUE;\r\nEND_IF\r\n\r\n(* This shall be called during online change and global init *)\r\nIoMgrUpdateMapping2(ADR(iotaskmap[0]), {2}, pszConfigApplication := {5});\r\n{6}\r\n";

		internal static readonly string GlobalExitPou_Name = "IoGlobalExit__Pou";

		private static readonly string GlobalExitPou_InterfaceCrc = "\r\n{{implicit}}\r\n{{attribute 'call_before_global_exit_slot':='100000'}}\r\n{{attribute 'crc_for_latelanguagemodel':='{0}'}}\r\nFUNCTION IoGlobalExit__Pou\r\nVAR\r\nEND_VAR\r\n";

		private static readonly string GlobalExitPou_Interface = "\r\n{implicit}\r\n{attribute 'call_before_global_exit_slot':='100000'}\r\nFUNCTION IoGlobalExit__Pou\r\nVAR\r\nEND_VAR\r\n";

		private static readonly string GlobalExitPou_Body = "\r\nIoMgrUpdateConfiguration(pConnectorList := 0, nCount := 0);\r\n{0}\r\n";

		internal static readonly string ErrorPou_Name = "IoConfig__ErrorPou";

		private static readonly string ErrorPou_Interface = "\r\n{attribute 'signature_flag' := '1073741824'}\r\n{implicit}\r\nPROGRAM IoConfig__ErrorPou\r\nVAR\r\n    pPointer: POINTER TO BYTE;\r\nEND_VAR\r\n";

		public static void WriteBeforeTaskPou(XmlWriter xmlWriter, ICompileContext refcon, Guid guidTask, int nTaskMapIndex, StartBusCycleInfo[] watchdogTriggers, string stLateInitCode, bool bUpdateInStop, bool bCycleControl, string stAdditionalCalls)
		{
			XmlAttribute[] attributes = new XmlAttribute[2]
			{
				new XmlAttribute("slot", "100"),
				new XmlAttribute("task-id", guidTask.ToString())
			};
			StringBuilder stringBuilder = new StringBuilder();
			foreach (StartBusCycleInfo startBusCycleInfo in watchdogTriggers)
			{
				if (startBusCycleInfo.BeforeReadInputs)
				{
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)40))
					{
						stringBuilder.Append("{IF defined (pou:IoMgrStartBusCycle2)}\n");
						stringBuilder.AppendFormat("IoMgrStartBusCycle2(ADR(moduleList[{0}]),BusCycleType.BCT_START);\n", startBusCycleInfo.ModuleId);
						stringBuilder.Append("{ELSE}\n");
						stringBuilder.AppendFormat("IoMgrStartBuscycle(ADR(moduleList[{0}]));\n", startBusCycleInfo.ModuleId);
						stringBuilder.Append("{END_IF}\n");
					}
					else
					{
						stringBuilder.AppendFormat("IoMgrStartBuscycle(ADR(moduleList[{0}]));\n", startBusCycleInfo.ModuleId);
					}
				}
			}
			string text = string.Format(BeforeTask_PouName, nTaskMapIndex);
			string stInterface = ((!bCycleControl) ? string.Format(BeforeTask_Interface_No_Cycle_Control, nTaskMapIndex) : string.Format(BeforeTask_Interface, nTaskMapIndex));
			string empty = string.Empty;
			empty = ((bUpdateInStop || !bCycleControl) ? BeforeTask_BodyUpdateInStop : ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)0)) ? BeforeTask_Body : BeforeTask_BodyNoBP));
			ISignature val = null;
			if (refcon != null)
			{
				val = ((ICompileContextCommon)refcon).GetSignature(text);
			}
			Guid guidPou = ((val == null) ? LanguageModelHelper.CreateDeterministicGuid(guidTask, text) : val.ObjectGuid);
			string stBody = string.Format(empty, stringBuilder.ToString(), nTaskMapIndex, stLateInitCode, stAdditionalCalls);
			WritePou(xmlWriter, guidPou, text, stInterface, stBody, attributes);
		}

		public static void WriteAfterTaskPou(XmlWriter xmlWriter, ICompileContext refcon, Guid guidTask, int nTaskMapIndex, StartBusCycleInfo[] busCycleInfos, DriverInfo driverInfo, bool bCycleControl, string stAdditionalCalls, StartBusCycleInfo[] allbusCycleInfos, string stTaskName)
		{
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			XmlAttribute[] attributes = new XmlAttribute[2]
			{
				new XmlAttribute("slot", "60000"),
				new XmlAttribute("task-id", guidTask.ToString())
			};
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StartBusCycleInfo[] array = busCycleInfos;
			foreach (StartBusCycleInfo startBusCycleInfo in array)
			{
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)10, (ushort)0) && startBusCycleInfo.ExternEvent && !flag)
				{
					flag = true;
					string eventName;
					string externalEventFBInstanceName = LanguageModelHelper.GetExternalEventFBInstanceName(stTaskName, out eventName);
					stringBuilder2.AppendFormat("{0}();\n", externalEventFBInstanceName);
				}
				if (startBusCycleInfo.AfterWriteOutputs)
				{
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)40))
					{
						stringBuilder.Append("{IF defined (pou:IoMgrStartBusCycle2)}\n");
						stringBuilder.AppendFormat("IoMgrStartBusCycle2(ADR(moduleList[{0}]),BusCycleType.BCT_END);\n", startBusCycleInfo.ModuleId);
						stringBuilder.Append("{ELSE}\n");
						stringBuilder.AppendFormat("IoMgrStartBuscycle(ADR(moduleList[{0}]));\n", startBusCycleInfo.ModuleId);
						stringBuilder.Append("{END_IF}\n");
					}
					else
					{
						stringBuilder.AppendFormat("IoMgrStartBuscycle(ADR(moduleList[{0}]));\n", startBusCycleInfo.ModuleId);
					}
				}
			}
			stAdditionalCalls += stringBuilder2.ToString();
			StringBuilder stringBuilder3 = new StringBuilder();
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
			{
				array = allbusCycleInfos;
				foreach (StartBusCycleInfo startBusCycleInfo2 in array)
				{
					if (startBusCycleInfo2.AfterWriteOutputs)
					{
						stringBuilder3.Append("{IF defined (pou:IoMgrStartBusCycle2)}\n");
						stringBuilder3.AppendFormat("IoMgrStartBusCycle2(ADR(moduleList[{0}]),BusCycleType.BCT_END);\n", startBusCycleInfo2.ModuleId);
						stringBuilder3.Append("{ELSE}\n");
						stringBuilder3.AppendFormat("IoMgrStartBuscycle(ADR(moduleList[{0}]));\n", startBusCycleInfo2.ModuleId);
						stringBuilder3.Append("{END_IF}\n");
					}
				}
			}
			string text = string.Format(AfterTask_PouName, nTaskMapIndex);
			string stInterface = ((!bCycleControl) ? string.Format(AfterTask_Interface_No_Cycle_Control, nTaskMapIndex) : (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0) ? string.Format(AfterTask_InterfaceNoGlitch_35110, nTaskMapIndex) : (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0) ? string.Format(AfterTask_InterfaceNoGlitch, nTaskMapIndex) : ((driverInfo.UpdateIOsInStop || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0)) ? string.Format(AfterTask_Interface, nTaskMapIndex) : string.Format(AfterTask_InterfaceNoGlitch, nTaskMapIndex)))));
			string empty = string.Empty;
			empty = ((driverInfo.UpdateIOsInStop || !bCycleControl) ? AfterTask_BodyInStop : (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0) ? (((int)driverInfo.StopResetBehaviourSetting != 0) ? AfterTask_BodyNoGlitchNoBP_35110 : AfterTask_BodyNoGlitchNoBPKeepCurrentValue_35110) : (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)0) ? AfterTask_BodyNoGlitchNoBP : ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0)) ? AfterTask_Body : AfterTask_BodyNoGlitch))));
			ISignature val = null;
			if (refcon != null)
			{
				val = ((ICompileContextCommon)refcon).GetSignature(text);
			}
			Guid guidPou = ((val == null) ? LanguageModelHelper.CreateDeterministicGuid(guidTask, text) : val.ObjectGuid);
			string stBody;
			if (bCycleControl && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
			{
				StringBuilder stringBuilder4 = new StringBuilder();
				if (stringBuilder3.Length > 0)
				{
					if (driverInfo.UpdateIOsInStop)
					{
						stringBuilder4.AppendLine("IF pApplicationInfo <> 0 THEN");
						stringBuilder4.AppendLine("udOpState := pApplicationInfo^.udOpState;");
						stringBuilder4.AppendLine("END_IF");
					}
					stringBuilder4.AppendLine("IF (udOpState AND 16#20) <> 0 OR (udOpState AND 16#10000) <> 0 THEN");
					stringBuilder4.Append(stringBuilder3);
					if (stringBuilder.Length > 0)
					{
						stringBuilder4.AppendLine("ELSE");
						stringBuilder4.Append(stringBuilder);
					}
					stringBuilder4.AppendLine("END_IF");
				}
				else
				{
					stringBuilder4 = stringBuilder;
				}
				stBody = string.Format(empty, nTaskMapIndex, stringBuilder4.ToString(), stAdditionalCalls);
			}
			else
			{
				stBody = string.Format(empty, nTaskMapIndex, stringBuilder.ToString(), stAdditionalCalls);
			}
			WritePou(xmlWriter, guidPou, text, stInterface, stBody, attributes);
		}

		public static void WriteResetOutputsPou(XmlWriter xmlWriter, ICompileContext refcon, Guid guidPou, Guid guidTask, int nTaskMapIndex, StopResetBehaviour resetbehaviour, string stUserProgram, StartBusCycleInfo[] busCycleInfos, string stCallsBeforeWrite, string stCallsAfterWrite, bool bUpdateIOsStop)
		{
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Invalid comparison between Unknown and I4
			XmlAttribute[] attributes = ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)0)) ? new XmlAttribute[2]
			{
				new XmlAttribute("slot", "60002"),
				new XmlAttribute("task-id", guidTask.ToString())
			} : new XmlAttribute[2]
			{
				new XmlAttribute("slot", "44998"),
				new XmlAttribute("task-id", guidTask.ToString())
			});
			string text = string.Format(ResetOutputsPouName, nTaskMapIndex);
			string stInterface = (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0) ? string.Format(ResetOutputs_InterfaceException64Bit, nTaskMapIndex) : (((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)5, (ushort)70) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)) && (!bUpdateIOsStop || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)60))) ? string.Format(ResetOutputs_Interface, nTaskMapIndex) : string.Format(ResetOutputs_InterfaceException, nTaskMapIndex)));
			StringBuilder stringBuilder = new StringBuilder();
			string empty = string.Empty;
			foreach (StartBusCycleInfo startBusCycleInfo in busCycleInfos)
			{
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)40))
				{
					stringBuilder.Append("{IF defined (pou:IoMgrStartBusCycle2)}\n");
					stringBuilder.AppendFormat("IoMgrStartBusCycle2(ADR(moduleList[{0}]),BusCycleType.BCT_END);\n", startBusCycleInfo.ModuleId);
					stringBuilder.Append("{ELSE}\n");
					stringBuilder.AppendFormat("IoMgrStartBuscycle(ADR(moduleList[{0}]));\n", startBusCycleInfo.ModuleId);
					stringBuilder.Append("{END_IF}\n");
				}
				else
				{
					stringBuilder.AppendFormat("IoMgrStartBuscycle(ADR(moduleList[{0}]));\n", startBusCycleInfo.ModuleId);
				}
			}
			if ((int)resetbehaviour == 1)
			{
				string empty2 = string.Empty;
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
				{
					empty2 = ResetOutputsDefault_UpdateIosInStop + ResetOutputsDefault_Prg64BitNoGlitch;
					empty = string.Format(empty2, nTaskMapIndex);
				}
				else
				{
					empty2 = ((bUpdateIOsStop && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)60)) ? ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0)) ? (ResetOutputsDefault_UpdateIosInStop + ResetOutputsDefault_Prg) : (ResetOutputsDefault_UpdateIosInStop + ResetOutputsDefault_Prg64Bit)) : (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0) ? (ResetOutputsDefault_UpdateIosInStop + ResetOutputsDefault_Prg64Bit) : ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)5, (ushort)70) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)) ? (ResetOutputsDefault_Old + ResetOutputsDefault_Prg) : (ResetOutputsDefault_New + ResetOutputsDefault_Prg))));
					empty = string.Format(empty2, stCallsBeforeWrite, nTaskMapIndex, stringBuilder, stCallsAfterWrite);
				}
			}
			else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)14, (ushort)0))
			{
				empty = string.Format(ResetOutputsProgram_BodyUpdateNoGlitchOnlyOnce, stUserProgram);
			}
			else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
			{
				empty = string.Format(ResetOutputsProgram_BodyUpdateNoGlitch, stUserProgram);
			}
			else
			{
				string empty3 = string.Empty;
				empty3 = ((bUpdateIOsStop && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)60)) ? ResetOutputsProgram_BodyUpdateIosInStop : ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0) && (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)5, (ushort)70) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0))) ? ResetOutputsProgram_Body : ResetOutputsProgram_BodyException));
				empty = string.Format(empty3, stUserProgram, stCallsBeforeWrite, nTaskMapIndex, stringBuilder, stCallsAfterWrite);
			}
			ISignature val = null;
			if (refcon != null)
			{
				val = ((ICompileContextCommon)refcon).GetSignature(text);
			}
			if (val != null)
			{
				guidPou = val.ObjectGuid;
			}
			WritePou(xmlWriter, guidPou, text, stInterface, empty, attributes);
		}

		private static bool OptimizeIoUpdate(IDeviceIdentification devid)
		{
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(devid);
			return LocalTargetSettings.OptimizeIoUpdate.GetBoolValue(targetSettingsById);
		}

		public static string WriteGlobalInitPou(XmlWriter xmlWriter, ICompileContext refcon, Guid guidTask, string stAddrModuleList, int nModulesCount, int nTaskCount, string stFbInitializations, string stPastUpdateConfigInitialization, string stApplication, string stAdditionalCalls, IDeviceIdentification devid)
		{
			XmlAttribute[] attributes = new XmlAttribute[1]
			{
				new XmlAttribute("online_change_slot", "60000")
			};
			string empty = string.Empty;
			bool flag = OptimizeIoUpdate(devid);
			empty = (string.IsNullOrEmpty(stApplication) ? ((!flag) ? string.Format(GlobalInitPou_Body, stAddrModuleList, nModulesCount, nTaskCount, stFbInitializations, stPastUpdateConfigInitialization, stAdditionalCalls) : string.Format(GlobalInitPou_Body_NoUpdateMapping, stAddrModuleList, nModulesCount, nTaskCount, stFbInitializations, stPastUpdateConfigInitialization, stAdditionalCalls)) : ((!flag) ? string.Format(GlobalInitPou_Body2, stAddrModuleList, nModulesCount, nTaskCount, stFbInitializations, stPastUpdateConfigInitialization, stApplication, stAdditionalCalls) : string.Format(GlobalInitPou_Body2_NoUpdateMapping, stAddrModuleList, nModulesCount, nTaskCount, stFbInitializations, stPastUpdateConfigInitialization, stAdditionalCalls)));
			if (xmlWriter != null)
			{
				string stBody = "\r\n{IF NOT defined (IoConfigLateInit)}\r\n" + empty + "\r\n{END_IF}\r\n";
				ISignature val = null;
				if (refcon != null)
				{
					val = ((ICompileContextCommon)refcon).GetSignature(GlobalInitPou_Name);
				}
				Guid guidPou = ((val == null) ? LanguageModelHelper.CreateDeterministicGuid(guidTask, GlobalInitPou_Name) : val.ObjectGuid);
				if (flag)
				{
					WritePou(xmlWriter, guidPou, GlobalInitPou_Name, GlobalInitPou_Interface_NoUpdateMapping, stBody, attributes);
				}
				else
				{
					WritePou(xmlWriter, guidPou, GlobalInitPou_Name, GlobalInitPou_Interface, stBody, attributes);
				}
			}
			return "\r\n{IF defined (IoConfigLateInit)}\r\n{warning '" + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WarningIoConfigLateInit") + "' show_compile}\r\nIF (NOT bIoConfigLateInitDone) AND (pApplicationInfo^.udState <> 2) AND (pApplicationInfo^.udState <> 3)THEN\r\n" + empty + "\r\n\tbIoConfigLateInitDone := TRUE;\r\nEND_IF\r\n{END_IF}\r\n";
		}

		public static void WriteGlobalExitPou(XmlWriter xmlWriter, ICompileContext refcon, Guid appGuid, int iNumberOfResetCalls, long lLateCrc)
		{
			string text = string.Empty;
			string format = "\r\nIoConfigResetOutputs_{0}(ptaskinfo := 0,pApplicationInfo :=0);\r\n\r\n";
			for (int i = 0; i < iNumberOfResetCalls; i++)
			{
				text += string.Format(format, i * 2 + 1);
			}
			string stBody = string.Format(GlobalExitPou_Body, text);
			ISignature val = null;
			if (refcon != null)
			{
				val = ((ICompileContextCommon)refcon).GetSignature(GlobalExitPou_Name);
			}
			Guid guidPou = ((val == null) ? LanguageModelHelper.CreateDeterministicGuid(appGuid, GlobalExitPou_Name) : val.ObjectGuid);
			string stInterface = GlobalExitPou_Interface;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
			{
				stInterface = string.Format(GlobalExitPou_InterfaceCrc, lLateCrc.ToString());
			}
			WritePou(xmlWriter, guidPou, GlobalExitPou_Name, stInterface, stBody, null);
		}

		public static void WriteIoConfigErrorPou(XmlWriter xmlWriter, Guid guidPou, string stContent)
		{
			WritePou(xmlWriter, guidPou, ErrorPou_Name, ErrorPou_Interface, stContent, new XmlAttribute[0]);
		}

		public static void WriteDatatype(XmlWriter xmlWriter, Guid guidPou, Guid ObjectGuid, string stName, string stInterface)
		{
			xmlWriter.WriteStartElement("data-type");
			xmlWriter.WriteAttributeString("id", guidPou.ToString());
			xmlWriter.WriteAttributeString("name", stName);
			xmlWriter.WriteAttributeString("object-id", ObjectGuid.ToString());
			xmlWriter.WriteElementString("interface", stInterface);
			xmlWriter.WriteEndElement();
		}

		public static void WritePou(XmlWriter xmlWriter, Guid guidPou, string stName, string stInterface, string stBody, XmlAttribute[] attributes)
		{
			if (guidPou == Guid.Empty)
			{
				Debug.Fail("The Guid for a GVL must not be null");
				throw new ArgumentException("The Guid for a GVL must not be null", "guidPou");
			}
			xmlWriter.WriteStartElement("pou");
			xmlWriter.WriteAttributeString("id", guidPou.ToString());
			xmlWriter.WriteAttributeString("name", stName);
			if (attributes != null)
			{
				foreach (XmlAttribute xmlAttribute in attributes)
				{
					xmlWriter.WriteAttributeString(xmlAttribute._stName, xmlAttribute._stValue);
				}
			}
			xmlWriter.WriteElementString("interface", stInterface);
			xmlWriter.WriteElementString("body", stBody);
			xmlWriter.WriteEndElement();
		}
	}
}
