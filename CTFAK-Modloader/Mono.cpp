
#include "Mono.h"
#include <iostream>

	mono_init_t Mono::mono_init = NULL;
	mono_jit_init_t Mono::mono_jit_init = NULL;
	mono_jit_init_version_t Mono::mono_jit_init_version = NULL;
	mono_jit_cleanup_t Mono::mono_jit_cleanup = NULL;
	mono_assembly_setrootdir_t Mono::mono_assembly_setrootdir = NULL;
	mono_set_assemblies_path_t Mono::mono_set_assemblies_path = NULL;
	mono_set_config_dir_t Mono::mono_set_config_dir = NULL;
	mono_domain_assembly_open_t Mono::mono_domain_assembly_open = NULL;
	mono_assembly_get_image_t Mono::mono_assembly_get_image = NULL;
	mono_class_from_name_t Mono::mono_class_from_name = NULL;
	mono_class_get_method_from_name_t Mono::mono_class_get_method_from_name = NULL;
	mono_runtime_invoke_t Mono::mono_runtime_invoke = NULL;
	mono_add_internal_call_t Mono::mono_add_internal_call = NULL;
	mono_thread_current_t Mono::mono_thread_current = NULL;
	mono_thread_set_main_t Mono::mono_thread_set_main = NULL;
	mono_string_to_utf8_t Mono::mono_string_to_utf8 = NULL;
	mono_string_new_t Mono::mono_string_new = NULL;
	mono_class_get_property_from_name_t Mono::mono_class_get_property_from_name = NULL;
	mono_property_get_get_method_t Mono::mono_property_get_get_method = NULL;
	mono_object_get_class_t Mono::mono_object_get_class = NULL;
	mono_runtime_set_main_args_t Mono::mono_runtime_set_main_args = NULL;
	mono_domain_set_config_t Mono::mono_domain_set_config = NULL;
	mono_method_get_name_t Mono::mono_method_get_name = NULL;
	void Mono::Initialize()
	{
		auto Module = LoadLibraryA("mono.dll");
		if (!Module)std::cout << "MONO MODULE NOT FOUND" << std::endl;

		mono_init = (mono_init_t)GetProcAddress(Module, "mono_init");
		mono_jit_init = (mono_jit_init_t)GetProcAddress(Module, "mono_jit_init");
		mono_jit_init_version = (mono_jit_init_version_t)GetProcAddress(Module, "mono_jit_init_version");
		mono_jit_cleanup = (mono_jit_cleanup_t)GetProcAddress(Module, "mono_jit_cleanup");
		mono_assembly_setrootdir = (mono_assembly_setrootdir_t)GetProcAddress(Module, "mono_assembly_setrootdir");
		mono_set_assemblies_path = (mono_set_assemblies_path_t)GetProcAddress(Module, "mono_set_assemblies_path");
		mono_set_config_dir = (mono_set_config_dir_t)GetProcAddress(Module, "mono_set_config_dir");
		mono_domain_assembly_open = (mono_domain_assembly_open_t)GetProcAddress(Module, "mono_domain_assembly_open");
		mono_assembly_get_image = (mono_assembly_get_image_t)GetProcAddress(Module, "mono_assembly_get_image");
		mono_class_from_name = (mono_class_from_name_t)GetProcAddress(Module, "mono_class_from_name");
		mono_class_get_method_from_name = (mono_class_get_method_from_name_t)GetProcAddress(Module, "mono_class_get_method_from_name");
		mono_runtime_invoke = (mono_runtime_invoke_t)GetProcAddress(Module, "mono_runtime_invoke");
		mono_method_get_name = (mono_method_get_name_t)GetProcAddress(Module, "mono_method_get_name");
		mono_add_internal_call = (mono_add_internal_call_t)GetProcAddress(Module, "mono_add_internal_call");
		mono_thread_current = (mono_thread_current_t)GetProcAddress(Module, "mono_thread_current");
		mono_thread_set_main = (mono_thread_set_main_t)GetProcAddress(Module, "mono_thread_set_main");
		mono_string_to_utf8 = (mono_string_to_utf8_t)GetProcAddress(Module, "mono_string_to_utf8");
		mono_string_new = (mono_string_new_t)GetProcAddress(Module, "mono_string_new");
		mono_class_get_property_from_name = (mono_class_get_property_from_name_t)GetProcAddress(Module, "mono_class_get_property_from_name");
		mono_property_get_get_method = (mono_property_get_get_method_t)GetProcAddress(Module, "mono_property_get_get_method");
		mono_object_get_class = (mono_object_get_class_t)GetProcAddress(Module, "mono_object_get_class");
	}