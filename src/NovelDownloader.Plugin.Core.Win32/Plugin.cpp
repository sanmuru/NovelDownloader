#include "stdafx.h"
#include "Plugin.h"

#ifdef __cplusplus
Plugin_::Plugin_()
{
	_Version version = { 0,0,0,NULL,NULL };
	_Version min_version = { 0,0,0,NULL,NULL };
	PluginInterface_ pi = {
		NULL,
		Name = NULL,
		DisplayName = NULL,
		Version = &version,
		MinVersion=&min_version,
		Description = NULL
	};
	this->inner_plugin = &pi;
	this->Name = pi.Name;
	this->DisplayName = pi.DisplayName;
	this->Version = pi.Version;
	this->MinVersion = pi.MinVersion;
	this->Description = pi.Description;
}

Plugin_::~Plugin_()
{
	if (this->inner_plugin != NULL)
	{
		PluginInterface_ *pi = (PluginInterface_*)(this->inner_plugin);
		pi->reserved_inner_plugin = NULL;
		pi->Name = NULL;
		pi->DisplayName = NULL;
		if (pi->Version != NULL)
		{
			::Version version = pi->Version;
			version->Date = NULL;
			version->Period = NULL;

			pi->Version = NULL;
		}
		if (pi->MinVersion != NULL)
		{
			::Version min_version = pi->MinVersion;
			min_version->Date = NULL;
			min_version->Period = NULL;

			pi->MinVersion = NULL;
		}
		pi->Description = NULL;
	}
	this->inner_plugin = NULL;
	this->Name = NULL;
	this->DisplayName = NULL;
	if (this->Version != NULL)
	{
		::Version version = this->Version;
		version->Date = NULL;
		version->Period = NULL;

		this->Version = NULL;
	}
	if (this->MinVersion != NULL)
	{
		::Version min_version = this->MinVersion;
		min_version->Date = NULL;
		min_version->Period = NULL;

		this->MinVersion = NULL;
	}
	this->Description = NULL;
}
#endif