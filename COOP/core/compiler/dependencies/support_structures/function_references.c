//
// Created by jradin on 3/21/2019.
//

#include <mem.h>
#include <math.h>
#include <stdarg.h>
#include <stdlib.h>
#include "function_references.h"
#include "Object.h"

struct function_info{
	char* name;
	int inputs;
	char** input_types;
	coop_function function;
};

typedef struct function_bucket{
	unsigned int length;
	Function_Info** infos;
} bucket;

struct function_references{
	int num_buckets;
	bucket* buckets;
};

char** EMPTY_LIST = NULL;


Function_Info *create_Function_Info(char *name, int inputs, char **input_type_names, coop_function function) {
	Function_Info* output = (Function_Info*) malloc(sizeof(Function_Info));
	output->name = name;
	output->inputs = inputs;
	output->input_types = (char**) malloc(sizeof(char*) * inputs);
	for (int i = 0; i < inputs; ++i) {
		output->input_types[i] = input_type_names[i];
	}
	output->function = function;
	return output;
}
Function_Info *create_Function_Info_Types(char *name, int inputs, type_info *input_type_infos, coop_function function) {
	char** strs = (char**) malloc(sizeof(char*) * inputs);
	for (int i = 0; i < inputs; ++i) {
		strs[i] = input_type_infos[i].name;
	}
	Function_Info* output = create_Function_Info(name, inputs, strs, function);
	free(strs);
	return output;
}

Function_References* create_Function_References(int size){
	Function_References* output = malloc(sizeof(Function_References));
	output->num_buckets = size;
	output->buckets = malloc(sizeof(bucket)*size);
	for (int i = 0; i < size; ++i) {
		output->buckets[i].length = 0;
		output->buckets[i].infos = malloc(0);
	}
	return output;
}

unsigned int name_hashing_function(int modulo, char *string){
	unsigned long sum = 0;
	size_t N = strlen(string);
	for (int i = 0; i < N; ++i) {
		sum += string[i] * (long) pow(32, N - i - 1);
	}
	
	return sum % modulo;
}

void add_to_function_map(Function_References* map, Function_Info* info){
	if(contains_function_info(map, info)) return;
	unsigned int hash = name_hashing_function(map->num_buckets, info->name);
	bucket *b = &map->buckets[hash];
	unsigned int new_size = b->length + 1;
	b->infos = (Function_Info**) realloc(b->infos, sizeof(Function_Info*)*new_size);
	b->infos[b->length] = info;
	b->length = new_size;
}

bool same_parameters(int length_a, char** a, int length_b, char** b){
	if(length_a != length_b) return false;
	for (int i = 0; i < length_a; ++i) {
		if(strcmp(a[i], b[i]) != 0) return false;
	}
	return true;
}

bool contains_function_info(Function_References *map, Function_Info *info) {
	unsigned int hash = name_hashing_function(map->num_buckets, info->name);
	bucket b = map->buckets[hash];
	for (int i = 0; i < b.length; ++i) {
		struct function_info b_info = *b.infos[i];
		if(!strcmp(b_info.name, info->name) &&
		same_parameters(info->inputs, info->input_types, b_info.inputs, b_info.input_types))
			return true;
	}
	
	return false;
}


Function_Info *get_function_info(Function_References *map, char *name, int num_inputs, char **input_type_infos) {
	unsigned int hash = name_hashing_function(map->num_buckets, name);
	bucket *b = &map->buckets[hash];
	for (int i = 0; i < b->length; ++i) {
		struct function_info *f = b->infos[i];
		if(!strcmp(f->name, name) &&
			same_parameters(f->inputs, f->input_types, num_inputs, input_type_infos)){
			return f;
		}
	}
	
	return NULL;
}


coop_function use_function(Function_Info* functionInfo){
	return functionInfo->function;
}

coop_function use_function_name_types(Function_References *map, char *name, int num_inputs, char **input_types) {
	Function_Info* info = get_function_info(map, name, num_inputs, input_types);
	if(info == NULL) exit(-1);
	return info->function;
}

coop_function get_function(Function_References *map, char* name, unsigned int inputs, ...){
	va_list args;
	va_start(args, inputs);
	char** params = malloc(sizeof(char*)*inputs);
	for (int i = 0; i < inputs; ++i) {
		char* type_name = va_arg(args, char*);
		params[i] = type_name;
	}
	coop_function output = use_function_name_types(map,name,inputs,params);
	free(params);
	return output;
}
