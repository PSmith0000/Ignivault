#define ICALL_TABLE_corlib 1

static int corlib_icall_indexes [] = {
216,
229,
230,
231,
232,
233,
234,
235,
236,
237,
240,
241,
242,
415,
416,
417,
445,
446,
447,
474,
475,
476,
593,
594,
595,
598,
635,
636,
637,
638,
639,
643,
645,
647,
649,
655,
663,
664,
665,
666,
667,
668,
669,
670,
671,
672,
673,
674,
675,
676,
677,
678,
679,
681,
682,
683,
684,
685,
686,
687,
780,
781,
782,
783,
784,
785,
786,
787,
788,
789,
790,
791,
792,
793,
794,
795,
796,
798,
799,
800,
801,
802,
803,
804,
866,
875,
876,
945,
951,
954,
956,
961,
962,
964,
965,
969,
970,
972,
973,
976,
977,
978,
981,
983,
986,
988,
990,
999,
1067,
1069,
1071,
1081,
1082,
1083,
1085,
1091,
1092,
1093,
1094,
1095,
1103,
1104,
1105,
1109,
1110,
1112,
1116,
1117,
1118,
1410,
1599,
1600,
10121,
10122,
10124,
10125,
10126,
10127,
10128,
10130,
10131,
10132,
10133,
10151,
10153,
10160,
10162,
10164,
10166,
10169,
10220,
10221,
10223,
10224,
10225,
10226,
10227,
10229,
10231,
11428,
11432,
11434,
11435,
11436,
11437,
11881,
11882,
11883,
11884,
11902,
11903,
11904,
11949,
12019,
12022,
12030,
12031,
12032,
12033,
12034,
12388,
12393,
12394,
12422,
12459,
12466,
12473,
12484,
12488,
12513,
12596,
12598,
12607,
12609,
12610,
12617,
12632,
12652,
12653,
12661,
12663,
12670,
12671,
12674,
12676,
12681,
12687,
12688,
12695,
12697,
12709,
12712,
12713,
12714,
12726,
12736,
12742,
12743,
12744,
12746,
12747,
12764,
12766,
12781,
12799,
12824,
12829,
12830,
12831,
12868,
12869,
13439,
13527,
13528,
13738,
13739,
13746,
13747,
13748,
13753,
13810,
14334,
14335,
14723,
14728,
14738,
15652,
15673,
15675,
15677,
};
void ves_icall_System_Array_InternalCreate (int,int,int,int,int);
int ves_icall_System_Array_GetCorElementTypeOfElementTypeInternal (int);
int ves_icall_System_Array_IsValueOfElementTypeInternal (int,int);
int ves_icall_System_Array_CanChangePrimitive (int,int,int);
int ves_icall_System_Array_FastCopy (int,int,int,int,int);
int ves_icall_System_Array_GetLengthInternal_raw (int,int,int);
int ves_icall_System_Array_GetLowerBoundInternal_raw (int,int,int);
void ves_icall_System_Array_GetGenericValue_icall (int,int,int);
void ves_icall_System_Array_GetValueImpl_raw (int,int,int,int);
void ves_icall_System_Array_SetGenericValue_icall (int,int,int);
void ves_icall_System_Array_SetValueImpl_raw (int,int,int,int);
void ves_icall_System_Array_InitializeInternal_raw (int,int);
void ves_icall_System_Array_SetValueRelaxedImpl_raw (int,int,int,int);
void ves_icall_System_Runtime_RuntimeImports_ZeroMemory (int,int);
void ves_icall_System_Runtime_RuntimeImports_Memmove (int,int,int);
void ves_icall_System_Buffer_BulkMoveWithWriteBarrier (int,int,int,int);
int ves_icall_System_Delegate_AllocDelegateLike_internal_raw (int,int);
int ves_icall_System_Delegate_CreateDelegate_internal_raw (int,int,int,int,int);
int ves_icall_System_Delegate_GetVirtualMethod_internal_raw (int,int);
void ves_icall_System_Enum_GetEnumValuesAndNames_raw (int,int,int,int);
int ves_icall_System_Enum_InternalGetCorElementType (int);
void ves_icall_System_Enum_InternalGetUnderlyingType_raw (int,int,int);
int ves_icall_System_Environment_get_ProcessorCount ();
int ves_icall_System_Environment_get_TickCount ();
int64_t ves_icall_System_Environment_get_TickCount64 ();
void ves_icall_System_Environment_FailFast_raw (int,int,int,int);
int ves_icall_System_GC_GetCollectionCount (int);
int ves_icall_System_GC_GetMaxGeneration ();
void ves_icall_System_GC_register_ephemeron_array_raw (int,int);
int ves_icall_System_GC_get_ephemeron_tombstone_raw (int);
int64_t ves_icall_System_GC_GetTotalAllocatedBytes_raw (int,int);
void ves_icall_System_GC_SuppressFinalize_raw (int,int);
void ves_icall_System_GC_ReRegisterForFinalize_raw (int,int);
void ves_icall_System_GC_GetGCMemoryInfo (int,int,int,int,int,int);
int ves_icall_System_GC_AllocPinnedArray_raw (int,int,int);
int ves_icall_System_Object_MemberwiseClone_raw (int,int);
double ves_icall_System_Math_Acos (double);
double ves_icall_System_Math_Acosh (double);
double ves_icall_System_Math_Asin (double);
double ves_icall_System_Math_Asinh (double);
double ves_icall_System_Math_Atan (double);
double ves_icall_System_Math_Atan2 (double,double);
double ves_icall_System_Math_Atanh (double);
double ves_icall_System_Math_Cbrt (double);
double ves_icall_System_Math_Ceiling (double);
double ves_icall_System_Math_Cos (double);
double ves_icall_System_Math_Cosh (double);
double ves_icall_System_Math_Exp (double);
double ves_icall_System_Math_Floor (double);
double ves_icall_System_Math_Log (double);
double ves_icall_System_Math_Log10 (double);
double ves_icall_System_Math_Pow (double,double);
double ves_icall_System_Math_Sin (double);
double ves_icall_System_Math_Sinh (double);
double ves_icall_System_Math_Sqrt (double);
double ves_icall_System_Math_Tan (double);
double ves_icall_System_Math_Tanh (double);
double ves_icall_System_Math_FusedMultiplyAdd (double,double,double);
double ves_icall_System_Math_Log2 (double);
double ves_icall_System_Math_ModF (double,int);
float ves_icall_System_MathF_Acos (float);
float ves_icall_System_MathF_Acosh (float);
float ves_icall_System_MathF_Asin (float);
float ves_icall_System_MathF_Asinh (float);
float ves_icall_System_MathF_Atan (float);
float ves_icall_System_MathF_Atan2 (float,float);
float ves_icall_System_MathF_Atanh (float);
float ves_icall_System_MathF_Cbrt (float);
float ves_icall_System_MathF_Ceiling (float);
float ves_icall_System_MathF_Cos (float);
float ves_icall_System_MathF_Cosh (float);
float ves_icall_System_MathF_Exp (float);
float ves_icall_System_MathF_Floor (float);
float ves_icall_System_MathF_Log (float);
float ves_icall_System_MathF_Log10 (float);
float ves_icall_System_MathF_Pow (float,float);
float ves_icall_System_MathF_Sin (float);
float ves_icall_System_MathF_Sinh (float);
float ves_icall_System_MathF_Sqrt (float);
float ves_icall_System_MathF_Tan (float);
float ves_icall_System_MathF_Tanh (float);
float ves_icall_System_MathF_FusedMultiplyAdd (float,float,float);
float ves_icall_System_MathF_Log2 (float);
float ves_icall_System_MathF_ModF (float,int);
int ves_icall_RuntimeMethodHandle_GetFunctionPointer_raw (int,int);
void ves_icall_RuntimeMethodHandle_ReboxFromNullable_raw (int,int,int);
void ves_icall_RuntimeMethodHandle_ReboxToNullable_raw (int,int,int,int);
int ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw (int,int,int);
void ves_icall_RuntimeType_make_array_type_raw (int,int,int,int);
void ves_icall_RuntimeType_make_byref_type_raw (int,int,int);
void ves_icall_RuntimeType_make_pointer_type_raw (int,int,int);
void ves_icall_RuntimeType_MakeGenericType_raw (int,int,int,int);
int ves_icall_RuntimeType_GetMethodsByName_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_GetPropertiesByName_native_raw (int,int,int,int,int);
int ves_icall_RuntimeType_GetConstructors_native_raw (int,int,int);
int ves_icall_System_RuntimeType_CreateInstanceInternal_raw (int,int);
void ves_icall_RuntimeType_GetDeclaringMethod_raw (int,int,int);
void ves_icall_System_RuntimeType_getFullName_raw (int,int,int,int,int);
void ves_icall_RuntimeType_GetGenericArgumentsInternal_raw (int,int,int,int);
int ves_icall_RuntimeType_GetGenericParameterPosition (int);
int ves_icall_RuntimeType_GetEvents_native_raw (int,int,int,int);
int ves_icall_RuntimeType_GetFields_native_raw (int,int,int,int,int);
void ves_icall_RuntimeType_GetInterfaces_raw (int,int,int);
int ves_icall_RuntimeType_GetNestedTypes_native_raw (int,int,int,int,int);
void ves_icall_RuntimeType_GetDeclaringType_raw (int,int,int);
void ves_icall_RuntimeType_GetName_raw (int,int,int);
void ves_icall_RuntimeType_GetNamespace_raw (int,int,int);
int ves_icall_RuntimeType_FunctionPointerReturnAndParameterTypes_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetAttributes (int);
int ves_icall_RuntimeTypeHandle_GetMetadataToken_raw (int,int);
void ves_icall_RuntimeTypeHandle_GetGenericTypeDefinition_impl_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_GetCorElementType (int);
int ves_icall_RuntimeTypeHandle_HasInstantiation (int);
int ves_icall_RuntimeTypeHandle_IsInstanceOfType_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_HasReferences_raw (int,int);
int ves_icall_RuntimeTypeHandle_GetArrayRank_raw (int,int);
void ves_icall_RuntimeTypeHandle_GetAssembly_raw (int,int,int);
void ves_icall_RuntimeTypeHandle_GetElementType_raw (int,int,int);
void ves_icall_RuntimeTypeHandle_GetModule_raw (int,int,int);
void ves_icall_RuntimeTypeHandle_GetBaseType_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_type_is_assignable_from_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_IsGenericTypeDefinition (int);
int ves_icall_RuntimeTypeHandle_GetGenericParameterInfo_raw (int,int);
int ves_icall_RuntimeTypeHandle_is_subclass_of_raw (int,int,int);
int ves_icall_RuntimeTypeHandle_IsByRefLike_raw (int,int);
void ves_icall_System_RuntimeTypeHandle_internal_from_name_raw (int,int,int,int,int,int);
int ves_icall_System_String_FastAllocateString_raw (int,int);
int ves_icall_System_String_InternalIsInterned_raw (int,int);
int ves_icall_System_String_InternalIntern_raw (int,int);
int ves_icall_System_Type_internal_from_handle_raw (int,int);
int ves_icall_System_ValueType_InternalGetHashCode_raw (int,int,int);
int ves_icall_System_ValueType_Equals_raw (int,int,int,int);
int ves_icall_System_Threading_Interlocked_CompareExchange_Int (int,int,int);
void ves_icall_System_Threading_Interlocked_CompareExchange_Object (int,int,int,int);
int ves_icall_System_Threading_Interlocked_Decrement_Int (int);
int ves_icall_System_Threading_Interlocked_Increment_Int (int);
int64_t ves_icall_System_Threading_Interlocked_Increment_Long (int);
int ves_icall_System_Threading_Interlocked_Exchange_Int (int,int);
void ves_icall_System_Threading_Interlocked_Exchange_Object (int,int,int);
int64_t ves_icall_System_Threading_Interlocked_CompareExchange_Long (int,int64_t,int64_t);
int64_t ves_icall_System_Threading_Interlocked_Exchange_Long (int,int64_t);
int ves_icall_System_Threading_Interlocked_Add_Int (int,int);
int64_t ves_icall_System_Threading_Interlocked_Add_Long (int,int64_t);
void ves_icall_System_Threading_Monitor_Monitor_Enter_raw (int,int);
void mono_monitor_exit_icall_raw (int,int);
void ves_icall_System_Threading_Monitor_Monitor_pulse_raw (int,int);
void ves_icall_System_Threading_Monitor_Monitor_pulse_all_raw (int,int);
int ves_icall_System_Threading_Monitor_Monitor_wait_raw (int,int,int,int);
void ves_icall_System_Threading_Monitor_Monitor_try_enter_with_atomic_var_raw (int,int,int,int,int);
int64_t ves_icall_System_Threading_Monitor_Monitor_get_lock_contention_count ();
void ves_icall_System_Threading_Thread_InitInternal_raw (int,int);
int ves_icall_System_Threading_Thread_GetCurrentThread ();
void ves_icall_System_Threading_InternalThread_Thread_free_internal_raw (int,int);
int ves_icall_System_Threading_Thread_GetState_raw (int,int);
void ves_icall_System_Threading_Thread_SetState_raw (int,int,int);
void ves_icall_System_Threading_Thread_ClrState_raw (int,int,int);
void ves_icall_System_Threading_Thread_SetName_icall_raw (int,int,int,int);
int ves_icall_System_Threading_Thread_YieldInternal ();
void ves_icall_System_Threading_Thread_SetPriority_raw (int,int,int);
void ves_icall_System_Runtime_Loader_AssemblyLoadContext_PrepareForAssemblyLoadContextRelease_raw (int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_GetLoadContextForAssembly_raw (int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFile_raw (int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalInitializeNativeALC_raw (int,int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFromStream_raw (int,int,int,int,int,int);
int ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalGetLoadedAssemblies_raw (int);
int ves_icall_System_GCHandle_InternalAlloc_raw (int,int,int);
void ves_icall_System_GCHandle_InternalFree_raw (int,int);
int ves_icall_System_GCHandle_InternalGet_raw (int,int);
void ves_icall_System_GCHandle_InternalSet_raw (int,int,int);
int ves_icall_System_Runtime_InteropServices_Marshal_GetLastPInvokeError ();
void ves_icall_System_Runtime_InteropServices_Marshal_SetLastPInvokeError (int);
void ves_icall_System_Runtime_InteropServices_Marshal_StructureToPtr_raw (int,int,int,int);
int ves_icall_System_Runtime_InteropServices_NativeLibrary_LoadByName_raw (int,int,int,int,int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InternalGetHashCode_raw (int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetObjectValue_raw (int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetUninitializedObjectInternal_raw (int,int);
void ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_raw (int,int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetSpanDataFrom_raw (int,int,int,int);
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_SufficientExecutionStack ();
int ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InternalBox_raw (int,int,int);
int ves_icall_System_Reflection_Assembly_GetEntryAssembly_raw (int);
int ves_icall_System_Reflection_Assembly_InternalLoad_raw (int,int,int,int);
int ves_icall_System_Reflection_Assembly_InternalGetType_raw (int,int,int,int,int,int);
int ves_icall_System_Reflection_AssemblyName_GetNativeName (int);
int ves_icall_MonoCustomAttrs_GetCustomAttributesInternal_raw (int,int,int,int);
int ves_icall_MonoCustomAttrs_GetCustomAttributesDataInternal_raw (int,int);
int ves_icall_MonoCustomAttrs_IsDefinedInternal_raw (int,int,int);
int ves_icall_System_Reflection_FieldInfo_internal_from_handle_type_raw (int,int,int);
int ves_icall_System_Reflection_FieldInfo_get_marshal_info_raw (int,int);
int ves_icall_System_Reflection_LoaderAllocatorScout_Destroy (int);
void ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceNames_raw (int,int,int);
void ves_icall_System_Reflection_RuntimeAssembly_GetExportedTypes_raw (int,int,int);
void ves_icall_System_Reflection_RuntimeAssembly_GetInfo_raw (int,int,int,int);
int ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceInternal_raw (int,int,int,int,int);
void ves_icall_System_Reflection_Assembly_GetManifestModuleInternal_raw (int,int,int);
void ves_icall_System_Reflection_RuntimeCustomAttributeData_ResolveArgumentsInternal_raw (int,int,int,int,int,int,int);
void ves_icall_RuntimeEventInfo_get_event_info_raw (int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_EventInfo_internal_from_handle_type_raw (int,int,int);
int ves_icall_RuntimeFieldInfo_ResolveType_raw (int,int);
int ves_icall_RuntimeFieldInfo_GetParentType_raw (int,int,int);
int ves_icall_RuntimeFieldInfo_GetFieldOffset_raw (int,int);
int ves_icall_RuntimeFieldInfo_GetValueInternal_raw (int,int,int);
void ves_icall_RuntimeFieldInfo_SetValueInternal_raw (int,int,int,int);
int ves_icall_RuntimeFieldInfo_GetRawConstantValue_raw (int,int);
int ves_icall_reflection_get_token_raw (int,int);
void ves_icall_get_method_info_raw (int,int,int);
int ves_icall_get_method_attributes (int);
int ves_icall_System_Reflection_MonoMethodInfo_get_parameter_info_raw (int,int,int);
int ves_icall_System_MonoMethodInfo_get_retval_marshal_raw (int,int);
int ves_icall_System_Reflection_RuntimeMethodInfo_GetMethodFromHandleInternalType_native_raw (int,int,int,int);
int ves_icall_RuntimeMethodInfo_get_name_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_base_method_raw (int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_InternalInvoke_raw (int,int,int,int,int);
void ves_icall_RuntimeMethodInfo_GetPInvoke_raw (int,int,int,int,int);
int ves_icall_RuntimeMethodInfo_MakeGenericMethod_impl_raw (int,int,int);
int ves_icall_RuntimeMethodInfo_GetGenericArguments_raw (int,int);
int ves_icall_RuntimeMethodInfo_GetGenericMethodDefinition_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_IsGenericMethodDefinition_raw (int,int);
int ves_icall_RuntimeMethodInfo_get_IsGenericMethod_raw (int,int);
void ves_icall_InvokeClassConstructor_raw (int,int);
int ves_icall_InternalInvoke_raw (int,int,int,int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_RuntimeModule_ResolveMethodToken_raw (int,int,int,int,int,int);
int ves_icall_RuntimeParameterInfo_GetTypeModifiers_raw (int,int,int,int,int,int);
void ves_icall_RuntimePropertyInfo_get_property_info_raw (int,int,int,int);
int ves_icall_RuntimePropertyInfo_GetTypeModifiers_raw (int,int,int,int);
int ves_icall_property_info_get_default_value_raw (int,int);
int ves_icall_reflection_get_token_raw (int,int);
int ves_icall_System_Reflection_RuntimePropertyInfo_internal_from_handle_type_raw (int,int,int);
void ves_icall_DynamicMethod_create_dynamic_method_raw (int,int,int,int,int);
void ves_icall_AssemblyBuilder_basic_init_raw (int,int);
void ves_icall_AssemblyBuilder_UpdateNativeCustomAttributes_raw (int,int);
void ves_icall_ModuleBuilder_basic_init_raw (int,int);
void ves_icall_ModuleBuilder_set_wrappers_type_raw (int,int,int);
int ves_icall_ModuleBuilder_getUSIndex_raw (int,int,int);
int ves_icall_ModuleBuilder_getToken_raw (int,int,int,int);
int ves_icall_ModuleBuilder_getMethodToken_raw (int,int,int,int);
void ves_icall_ModuleBuilder_RegisterToken_raw (int,int,int,int);
int ves_icall_TypeBuilder_create_runtime_class_raw (int,int);
int ves_icall_System_IO_Stream_HasOverriddenBeginEndRead_raw (int,int);
int ves_icall_System_IO_Stream_HasOverriddenBeginEndWrite_raw (int,int);
int ves_icall_System_Diagnostics_Debugger_IsAttached_internal ();
int ves_icall_System_Diagnostics_StackFrame_GetFrameInfo (int,int,int,int,int,int,int,int);
void ves_icall_System_Diagnostics_StackTrace_GetTrace (int,int,int,int);
int ves_icall_Mono_RuntimeClassHandle_GetTypeFromClass (int);
void ves_icall_Mono_RuntimeGPtrArrayHandle_GPtrArrayFree (int);
int ves_icall_Mono_SafeStringMarshal_StringToUtf8 (int);
void ves_icall_Mono_SafeStringMarshal_GFree (int);
static void *corlib_icall_funcs [] = {
// token 216,
ves_icall_System_Array_InternalCreate,
// token 229,
ves_icall_System_Array_GetCorElementTypeOfElementTypeInternal,
// token 230,
ves_icall_System_Array_IsValueOfElementTypeInternal,
// token 231,
ves_icall_System_Array_CanChangePrimitive,
// token 232,
ves_icall_System_Array_FastCopy,
// token 233,
ves_icall_System_Array_GetLengthInternal_raw,
// token 234,
ves_icall_System_Array_GetLowerBoundInternal_raw,
// token 235,
ves_icall_System_Array_GetGenericValue_icall,
// token 236,
ves_icall_System_Array_GetValueImpl_raw,
// token 237,
ves_icall_System_Array_SetGenericValue_icall,
// token 240,
ves_icall_System_Array_SetValueImpl_raw,
// token 241,
ves_icall_System_Array_InitializeInternal_raw,
// token 242,
ves_icall_System_Array_SetValueRelaxedImpl_raw,
// token 415,
ves_icall_System_Runtime_RuntimeImports_ZeroMemory,
// token 416,
ves_icall_System_Runtime_RuntimeImports_Memmove,
// token 417,
ves_icall_System_Buffer_BulkMoveWithWriteBarrier,
// token 445,
ves_icall_System_Delegate_AllocDelegateLike_internal_raw,
// token 446,
ves_icall_System_Delegate_CreateDelegate_internal_raw,
// token 447,
ves_icall_System_Delegate_GetVirtualMethod_internal_raw,
// token 474,
ves_icall_System_Enum_GetEnumValuesAndNames_raw,
// token 475,
ves_icall_System_Enum_InternalGetCorElementType,
// token 476,
ves_icall_System_Enum_InternalGetUnderlyingType_raw,
// token 593,
ves_icall_System_Environment_get_ProcessorCount,
// token 594,
ves_icall_System_Environment_get_TickCount,
// token 595,
ves_icall_System_Environment_get_TickCount64,
// token 598,
ves_icall_System_Environment_FailFast_raw,
// token 635,
ves_icall_System_GC_GetCollectionCount,
// token 636,
ves_icall_System_GC_GetMaxGeneration,
// token 637,
ves_icall_System_GC_register_ephemeron_array_raw,
// token 638,
ves_icall_System_GC_get_ephemeron_tombstone_raw,
// token 639,
ves_icall_System_GC_GetTotalAllocatedBytes_raw,
// token 643,
ves_icall_System_GC_SuppressFinalize_raw,
// token 645,
ves_icall_System_GC_ReRegisterForFinalize_raw,
// token 647,
ves_icall_System_GC_GetGCMemoryInfo,
// token 649,
ves_icall_System_GC_AllocPinnedArray_raw,
// token 655,
ves_icall_System_Object_MemberwiseClone_raw,
// token 663,
ves_icall_System_Math_Acos,
// token 664,
ves_icall_System_Math_Acosh,
// token 665,
ves_icall_System_Math_Asin,
// token 666,
ves_icall_System_Math_Asinh,
// token 667,
ves_icall_System_Math_Atan,
// token 668,
ves_icall_System_Math_Atan2,
// token 669,
ves_icall_System_Math_Atanh,
// token 670,
ves_icall_System_Math_Cbrt,
// token 671,
ves_icall_System_Math_Ceiling,
// token 672,
ves_icall_System_Math_Cos,
// token 673,
ves_icall_System_Math_Cosh,
// token 674,
ves_icall_System_Math_Exp,
// token 675,
ves_icall_System_Math_Floor,
// token 676,
ves_icall_System_Math_Log,
// token 677,
ves_icall_System_Math_Log10,
// token 678,
ves_icall_System_Math_Pow,
// token 679,
ves_icall_System_Math_Sin,
// token 681,
ves_icall_System_Math_Sinh,
// token 682,
ves_icall_System_Math_Sqrt,
// token 683,
ves_icall_System_Math_Tan,
// token 684,
ves_icall_System_Math_Tanh,
// token 685,
ves_icall_System_Math_FusedMultiplyAdd,
// token 686,
ves_icall_System_Math_Log2,
// token 687,
ves_icall_System_Math_ModF,
// token 780,
ves_icall_System_MathF_Acos,
// token 781,
ves_icall_System_MathF_Acosh,
// token 782,
ves_icall_System_MathF_Asin,
// token 783,
ves_icall_System_MathF_Asinh,
// token 784,
ves_icall_System_MathF_Atan,
// token 785,
ves_icall_System_MathF_Atan2,
// token 786,
ves_icall_System_MathF_Atanh,
// token 787,
ves_icall_System_MathF_Cbrt,
// token 788,
ves_icall_System_MathF_Ceiling,
// token 789,
ves_icall_System_MathF_Cos,
// token 790,
ves_icall_System_MathF_Cosh,
// token 791,
ves_icall_System_MathF_Exp,
// token 792,
ves_icall_System_MathF_Floor,
// token 793,
ves_icall_System_MathF_Log,
// token 794,
ves_icall_System_MathF_Log10,
// token 795,
ves_icall_System_MathF_Pow,
// token 796,
ves_icall_System_MathF_Sin,
// token 798,
ves_icall_System_MathF_Sinh,
// token 799,
ves_icall_System_MathF_Sqrt,
// token 800,
ves_icall_System_MathF_Tan,
// token 801,
ves_icall_System_MathF_Tanh,
// token 802,
ves_icall_System_MathF_FusedMultiplyAdd,
// token 803,
ves_icall_System_MathF_Log2,
// token 804,
ves_icall_System_MathF_ModF,
// token 866,
ves_icall_RuntimeMethodHandle_GetFunctionPointer_raw,
// token 875,
ves_icall_RuntimeMethodHandle_ReboxFromNullable_raw,
// token 876,
ves_icall_RuntimeMethodHandle_ReboxToNullable_raw,
// token 945,
ves_icall_RuntimeType_GetCorrespondingInflatedMethod_raw,
// token 951,
ves_icall_RuntimeType_make_array_type_raw,
// token 954,
ves_icall_RuntimeType_make_byref_type_raw,
// token 956,
ves_icall_RuntimeType_make_pointer_type_raw,
// token 961,
ves_icall_RuntimeType_MakeGenericType_raw,
// token 962,
ves_icall_RuntimeType_GetMethodsByName_native_raw,
// token 964,
ves_icall_RuntimeType_GetPropertiesByName_native_raw,
// token 965,
ves_icall_RuntimeType_GetConstructors_native_raw,
// token 969,
ves_icall_System_RuntimeType_CreateInstanceInternal_raw,
// token 970,
ves_icall_RuntimeType_GetDeclaringMethod_raw,
// token 972,
ves_icall_System_RuntimeType_getFullName_raw,
// token 973,
ves_icall_RuntimeType_GetGenericArgumentsInternal_raw,
// token 976,
ves_icall_RuntimeType_GetGenericParameterPosition,
// token 977,
ves_icall_RuntimeType_GetEvents_native_raw,
// token 978,
ves_icall_RuntimeType_GetFields_native_raw,
// token 981,
ves_icall_RuntimeType_GetInterfaces_raw,
// token 983,
ves_icall_RuntimeType_GetNestedTypes_native_raw,
// token 986,
ves_icall_RuntimeType_GetDeclaringType_raw,
// token 988,
ves_icall_RuntimeType_GetName_raw,
// token 990,
ves_icall_RuntimeType_GetNamespace_raw,
// token 999,
ves_icall_RuntimeType_FunctionPointerReturnAndParameterTypes_raw,
// token 1067,
ves_icall_RuntimeTypeHandle_GetAttributes,
// token 1069,
ves_icall_RuntimeTypeHandle_GetMetadataToken_raw,
// token 1071,
ves_icall_RuntimeTypeHandle_GetGenericTypeDefinition_impl_raw,
// token 1081,
ves_icall_RuntimeTypeHandle_GetCorElementType,
// token 1082,
ves_icall_RuntimeTypeHandle_HasInstantiation,
// token 1083,
ves_icall_RuntimeTypeHandle_IsInstanceOfType_raw,
// token 1085,
ves_icall_RuntimeTypeHandle_HasReferences_raw,
// token 1091,
ves_icall_RuntimeTypeHandle_GetArrayRank_raw,
// token 1092,
ves_icall_RuntimeTypeHandle_GetAssembly_raw,
// token 1093,
ves_icall_RuntimeTypeHandle_GetElementType_raw,
// token 1094,
ves_icall_RuntimeTypeHandle_GetModule_raw,
// token 1095,
ves_icall_RuntimeTypeHandle_GetBaseType_raw,
// token 1103,
ves_icall_RuntimeTypeHandle_type_is_assignable_from_raw,
// token 1104,
ves_icall_RuntimeTypeHandle_IsGenericTypeDefinition,
// token 1105,
ves_icall_RuntimeTypeHandle_GetGenericParameterInfo_raw,
// token 1109,
ves_icall_RuntimeTypeHandle_is_subclass_of_raw,
// token 1110,
ves_icall_RuntimeTypeHandle_IsByRefLike_raw,
// token 1112,
ves_icall_System_RuntimeTypeHandle_internal_from_name_raw,
// token 1116,
ves_icall_System_String_FastAllocateString_raw,
// token 1117,
ves_icall_System_String_InternalIsInterned_raw,
// token 1118,
ves_icall_System_String_InternalIntern_raw,
// token 1410,
ves_icall_System_Type_internal_from_handle_raw,
// token 1599,
ves_icall_System_ValueType_InternalGetHashCode_raw,
// token 1600,
ves_icall_System_ValueType_Equals_raw,
// token 10121,
ves_icall_System_Threading_Interlocked_CompareExchange_Int,
// token 10122,
ves_icall_System_Threading_Interlocked_CompareExchange_Object,
// token 10124,
ves_icall_System_Threading_Interlocked_Decrement_Int,
// token 10125,
ves_icall_System_Threading_Interlocked_Increment_Int,
// token 10126,
ves_icall_System_Threading_Interlocked_Increment_Long,
// token 10127,
ves_icall_System_Threading_Interlocked_Exchange_Int,
// token 10128,
ves_icall_System_Threading_Interlocked_Exchange_Object,
// token 10130,
ves_icall_System_Threading_Interlocked_CompareExchange_Long,
// token 10131,
ves_icall_System_Threading_Interlocked_Exchange_Long,
// token 10132,
ves_icall_System_Threading_Interlocked_Add_Int,
// token 10133,
ves_icall_System_Threading_Interlocked_Add_Long,
// token 10151,
ves_icall_System_Threading_Monitor_Monitor_Enter_raw,
// token 10153,
mono_monitor_exit_icall_raw,
// token 10160,
ves_icall_System_Threading_Monitor_Monitor_pulse_raw,
// token 10162,
ves_icall_System_Threading_Monitor_Monitor_pulse_all_raw,
// token 10164,
ves_icall_System_Threading_Monitor_Monitor_wait_raw,
// token 10166,
ves_icall_System_Threading_Monitor_Monitor_try_enter_with_atomic_var_raw,
// token 10169,
ves_icall_System_Threading_Monitor_Monitor_get_lock_contention_count,
// token 10220,
ves_icall_System_Threading_Thread_InitInternal_raw,
// token 10221,
ves_icall_System_Threading_Thread_GetCurrentThread,
// token 10223,
ves_icall_System_Threading_InternalThread_Thread_free_internal_raw,
// token 10224,
ves_icall_System_Threading_Thread_GetState_raw,
// token 10225,
ves_icall_System_Threading_Thread_SetState_raw,
// token 10226,
ves_icall_System_Threading_Thread_ClrState_raw,
// token 10227,
ves_icall_System_Threading_Thread_SetName_icall_raw,
// token 10229,
ves_icall_System_Threading_Thread_YieldInternal,
// token 10231,
ves_icall_System_Threading_Thread_SetPriority_raw,
// token 11428,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_PrepareForAssemblyLoadContextRelease_raw,
// token 11432,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_GetLoadContextForAssembly_raw,
// token 11434,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFile_raw,
// token 11435,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalInitializeNativeALC_raw,
// token 11436,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalLoadFromStream_raw,
// token 11437,
ves_icall_System_Runtime_Loader_AssemblyLoadContext_InternalGetLoadedAssemblies_raw,
// token 11881,
ves_icall_System_GCHandle_InternalAlloc_raw,
// token 11882,
ves_icall_System_GCHandle_InternalFree_raw,
// token 11883,
ves_icall_System_GCHandle_InternalGet_raw,
// token 11884,
ves_icall_System_GCHandle_InternalSet_raw,
// token 11902,
ves_icall_System_Runtime_InteropServices_Marshal_GetLastPInvokeError,
// token 11903,
ves_icall_System_Runtime_InteropServices_Marshal_SetLastPInvokeError,
// token 11904,
ves_icall_System_Runtime_InteropServices_Marshal_StructureToPtr_raw,
// token 11949,
ves_icall_System_Runtime_InteropServices_NativeLibrary_LoadByName_raw,
// token 12019,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InternalGetHashCode_raw,
// token 12022,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetObjectValue_raw,
// token 12030,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetUninitializedObjectInternal_raw,
// token 12031,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InitializeArray_raw,
// token 12032,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_GetSpanDataFrom_raw,
// token 12033,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_SufficientExecutionStack,
// token 12034,
ves_icall_System_Runtime_CompilerServices_RuntimeHelpers_InternalBox_raw,
// token 12388,
ves_icall_System_Reflection_Assembly_GetEntryAssembly_raw,
// token 12393,
ves_icall_System_Reflection_Assembly_InternalLoad_raw,
// token 12394,
ves_icall_System_Reflection_Assembly_InternalGetType_raw,
// token 12422,
ves_icall_System_Reflection_AssemblyName_GetNativeName,
// token 12459,
ves_icall_MonoCustomAttrs_GetCustomAttributesInternal_raw,
// token 12466,
ves_icall_MonoCustomAttrs_GetCustomAttributesDataInternal_raw,
// token 12473,
ves_icall_MonoCustomAttrs_IsDefinedInternal_raw,
// token 12484,
ves_icall_System_Reflection_FieldInfo_internal_from_handle_type_raw,
// token 12488,
ves_icall_System_Reflection_FieldInfo_get_marshal_info_raw,
// token 12513,
ves_icall_System_Reflection_LoaderAllocatorScout_Destroy,
// token 12596,
ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceNames_raw,
// token 12598,
ves_icall_System_Reflection_RuntimeAssembly_GetExportedTypes_raw,
// token 12607,
ves_icall_System_Reflection_RuntimeAssembly_GetInfo_raw,
// token 12609,
ves_icall_System_Reflection_RuntimeAssembly_GetManifestResourceInternal_raw,
// token 12610,
ves_icall_System_Reflection_Assembly_GetManifestModuleInternal_raw,
// token 12617,
ves_icall_System_Reflection_RuntimeCustomAttributeData_ResolveArgumentsInternal_raw,
// token 12632,
ves_icall_RuntimeEventInfo_get_event_info_raw,
// token 12652,
ves_icall_reflection_get_token_raw,
// token 12653,
ves_icall_System_Reflection_EventInfo_internal_from_handle_type_raw,
// token 12661,
ves_icall_RuntimeFieldInfo_ResolveType_raw,
// token 12663,
ves_icall_RuntimeFieldInfo_GetParentType_raw,
// token 12670,
ves_icall_RuntimeFieldInfo_GetFieldOffset_raw,
// token 12671,
ves_icall_RuntimeFieldInfo_GetValueInternal_raw,
// token 12674,
ves_icall_RuntimeFieldInfo_SetValueInternal_raw,
// token 12676,
ves_icall_RuntimeFieldInfo_GetRawConstantValue_raw,
// token 12681,
ves_icall_reflection_get_token_raw,
// token 12687,
ves_icall_get_method_info_raw,
// token 12688,
ves_icall_get_method_attributes,
// token 12695,
ves_icall_System_Reflection_MonoMethodInfo_get_parameter_info_raw,
// token 12697,
ves_icall_System_MonoMethodInfo_get_retval_marshal_raw,
// token 12709,
ves_icall_System_Reflection_RuntimeMethodInfo_GetMethodFromHandleInternalType_native_raw,
// token 12712,
ves_icall_RuntimeMethodInfo_get_name_raw,
// token 12713,
ves_icall_RuntimeMethodInfo_get_base_method_raw,
// token 12714,
ves_icall_reflection_get_token_raw,
// token 12726,
ves_icall_InternalInvoke_raw,
// token 12736,
ves_icall_RuntimeMethodInfo_GetPInvoke_raw,
// token 12742,
ves_icall_RuntimeMethodInfo_MakeGenericMethod_impl_raw,
// token 12743,
ves_icall_RuntimeMethodInfo_GetGenericArguments_raw,
// token 12744,
ves_icall_RuntimeMethodInfo_GetGenericMethodDefinition_raw,
// token 12746,
ves_icall_RuntimeMethodInfo_get_IsGenericMethodDefinition_raw,
// token 12747,
ves_icall_RuntimeMethodInfo_get_IsGenericMethod_raw,
// token 12764,
ves_icall_InvokeClassConstructor_raw,
// token 12766,
ves_icall_InternalInvoke_raw,
// token 12781,
ves_icall_reflection_get_token_raw,
// token 12799,
ves_icall_System_Reflection_RuntimeModule_ResolveMethodToken_raw,
// token 12824,
ves_icall_RuntimeParameterInfo_GetTypeModifiers_raw,
// token 12829,
ves_icall_RuntimePropertyInfo_get_property_info_raw,
// token 12830,
ves_icall_RuntimePropertyInfo_GetTypeModifiers_raw,
// token 12831,
ves_icall_property_info_get_default_value_raw,
// token 12868,
ves_icall_reflection_get_token_raw,
// token 12869,
ves_icall_System_Reflection_RuntimePropertyInfo_internal_from_handle_type_raw,
// token 13439,
ves_icall_DynamicMethod_create_dynamic_method_raw,
// token 13527,
ves_icall_AssemblyBuilder_basic_init_raw,
// token 13528,
ves_icall_AssemblyBuilder_UpdateNativeCustomAttributes_raw,
// token 13738,
ves_icall_ModuleBuilder_basic_init_raw,
// token 13739,
ves_icall_ModuleBuilder_set_wrappers_type_raw,
// token 13746,
ves_icall_ModuleBuilder_getUSIndex_raw,
// token 13747,
ves_icall_ModuleBuilder_getToken_raw,
// token 13748,
ves_icall_ModuleBuilder_getMethodToken_raw,
// token 13753,
ves_icall_ModuleBuilder_RegisterToken_raw,
// token 13810,
ves_icall_TypeBuilder_create_runtime_class_raw,
// token 14334,
ves_icall_System_IO_Stream_HasOverriddenBeginEndRead_raw,
// token 14335,
ves_icall_System_IO_Stream_HasOverriddenBeginEndWrite_raw,
// token 14723,
ves_icall_System_Diagnostics_Debugger_IsAttached_internal,
// token 14728,
ves_icall_System_Diagnostics_StackFrame_GetFrameInfo,
// token 14738,
ves_icall_System_Diagnostics_StackTrace_GetTrace,
// token 15652,
ves_icall_Mono_RuntimeClassHandle_GetTypeFromClass,
// token 15673,
ves_icall_Mono_RuntimeGPtrArrayHandle_GPtrArrayFree,
// token 15675,
ves_icall_Mono_SafeStringMarshal_StringToUtf8,
// token 15677,
ves_icall_Mono_SafeStringMarshal_GFree,
};
static uint8_t corlib_icall_flags [] = {
0,
0,
0,
0,
0,
4,
4,
0,
4,
0,
4,
4,
4,
0,
0,
0,
4,
4,
4,
4,
0,
4,
0,
0,
0,
4,
0,
0,
4,
4,
4,
4,
4,
0,
4,
4,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
0,
4,
4,
4,
4,
4,
4,
4,
4,
0,
4,
4,
0,
0,
4,
4,
4,
4,
4,
4,
4,
4,
0,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
4,
4,
4,
4,
4,
4,
0,
4,
0,
4,
4,
4,
4,
4,
0,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
0,
0,
4,
4,
4,
4,
4,
4,
4,
0,
4,
4,
4,
4,
0,
4,
4,
4,
4,
4,
0,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
0,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
4,
0,
0,
0,
0,
0,
0,
0,
};
