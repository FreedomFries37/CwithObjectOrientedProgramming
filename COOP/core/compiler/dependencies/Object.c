#include "Object.h"
#include "String.h"
#include <mem.h>

struct Object* __init__Object(struct Object* object){
	*object->external_use->ptr = object;
	return object;
}

struct String* toStringObject(struct Object*){
	char* str = (char*) calloc(11,sizeof(char));
	sprintf("0x%x", object->external_use->ptr)
}