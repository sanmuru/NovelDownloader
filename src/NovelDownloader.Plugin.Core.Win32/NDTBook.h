#pragma once

#ifndef _NDTBOOK_H_
#define _NDTBOOK_H_

#include "exports.h"
#include "NDToken.h"

struct NDTBookInterface_;
struct NDTBook_;

#ifdef __cplusplus
typedef NDTBook_ NDTBook;
#else
typedef const struct NDTBookInterface_ *NDTBook;
#endif
typedef HANDLE HNDTBook;

struct NDTBookInterface_
{
	
};

struct NDTBook_
{

};

#endif