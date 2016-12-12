#pragma once

#ifndef _MACROS_H_
#define _MACROS_H_

#define EMPTY_STRING ""
#define DEFAULT_VERSION { 0,0,0,NULL,NULL }
#define DEFAULT_GUID { 0,0,0,{ '0','0','0','0' } }

#ifdef __cplusplus
#define SYNTAX_FILTER_ACCESS(target_name) (target_name)
#else
#define SYNTAX_FILTER_ACCESS(target_name) (*(target_name))
#endif




#define MALLOC(type_name, param_name, size) type_name *param_name = (type_name*)malloc(size);
#define FREE(type_name, param_name) free(param_name); param_name = (type_name*)NULL;

#define POINTER_ADDRESS_EQUALS(left, right) ((unsigned int)(left) == (unsigned int)(right))




#define COLLECTION_FIELDS(struct_name, field_name)\
static int INTERNAL_##struct_name##_##field_name##_count = 0;\
static int INTERNAL_##struct_name##_##field_name##_capacity = 0;

#define COLLECTION_ADD(struct_name, field_name, destinate_type, destinate_param, element_type, element_param, ADD_FUNC_NAME)\
static int INTERNAL_##struct_name##_##field_name##_##ADD_FUNC_NAME##(destinate_type *destinate_param, element_type *element_param)\
{\
	if (destinate_param == NULL || element_param == NULL) return;\
\
	if (INTERNAL_##struct_name##_##field_name##_count >= INTERNAL_##struct_name##_##field_name##_capacity)\
	{\
		size_t size = sizeof(element_type);\
		MALLOC(element_type, collection, size * (INTERNAL_##struct_name##_##field_name##_count + 10))\
			if (destinate_param->field_name != NULL)\
			{\
				memcpy_s(collection, size * INTERNAL_##struct_name##_##field_name##_count, destinate_param->field_name, size * INTERNAL_##struct_name##_##field_name##_count);\
				FREE(element_type, destinate_param->field_name)\
			}\
		destinate_param->field_name = collection;\
		FREE(element_type, collection);\
		INTERNAL_##struct_name##_##field_name##_capacity = INTERNAL_##struct_name##_##field_name##_count + 10;\
	}\
	destinate_param->field_name[INTERNAL_##struct_name##_##field_name##_count] = *element_param;\
	INTERNAL_##struct_name##_##field_name##_count++;\
	return INTERNAL_##struct_name##_##field_name##_count;\
}

#define COLLECTION_REMOVE(struct_name, field_name, destinate_type, destinate_param, element_type, element_param, REMOVE_FUNC_NAME)\
static bool INTERNAL_##struct_name##_##field_name##_##REMOVE_FUNC_NAME##(destinate_type *destinate_param, element_type *element_param)\
{\
	if (destinate_param == NULL || element_param == NULL) return;\
\
	for (int i = 0; i < INTERNAL_##struct_name##_##field_name##_count; i++)\
	{\
		if (POINTER_ADDRESS_EQUALS(&(destinate_param->field_name[i]), element_param))\
		{\
			size_t size = sizeof(element_type) * (INTERNAL_##struct_name##_##field_name##_count - i - 1);\
			MALLOC(element_type, temp, size)\
				memcpy_s(&temp[0], size, &(destinate_param->field_name[i + 1]), size);\
			memcpy_s(&(destinate_param->field_name[i]), size, &temp[0], size);\
			FREE(element_type, temp);\
			INTERNAL_##struct_name##_##field_name##_count--;\
			\
			return true;\
		}\
	}\
\
	return false;\
}

#endif