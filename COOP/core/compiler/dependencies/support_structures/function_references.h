//
// Created by jradin on 3/21/2019.
//


#ifndef COOPTESTING_FUNCTION_MAP_H
#define COOPTESTING_FUNCTION_MAP_H


#include "Object.h"
#include <stdbool.h>

char** EMPTY_LIST;

typedef struct Object* (*coop_function) ();

typedef struct function_info Function_Info;

typedef struct function_references Function_References;

Function_Info *create_Function_Info(char *name, int inputs, char **input_type_names, coop_function function);
Function_Info *create_Function_Info_Types(char *name, int inputs, type_info *input_type_infos, coop_function function);

Function_References* create_Function_References(int size);

void add_to_function_map(Function_References* map, Function_Info* info);

bool contains_function_info(Function_References *map, Function_Info *info);

Function_Info *get_function_info(Function_References *map, char *name, int num_inputs, char **input_type_infos);

coop_function use_function(Function_Info* functionInfo);
coop_function use_function_name_types(Function_References *map, char *name, int num_inputs, char **input_types);

coop_function get_function(Function_References *map, char* name, unsigned int inputs, ...);


#endif //COOPTESTING_FUNCTION_MAP_H
