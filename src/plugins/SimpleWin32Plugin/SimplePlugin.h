#pragma once

#include "ndplugin.impl.h"

class RandomBook :
    public Book
{
protected:
    bool initiated = false;
public:
    virtual void init();
};

class RandomVolume :
    public Volume
{
protected:
    bool initiated = false;
public:
    virtual void init();
};

class RandomChapter :
    public Chapter
{
protected:
    bool initiated = false;
public:
    virtual void init();
};