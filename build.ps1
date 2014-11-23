Param(
    [Parameter(Position=1,Mandatory=0)]
    [string[]]$task_list = @(),
    
    [Parameter(Position=2,Mandatory=0)]
    [string]$version
)

$psake_location = ".\packages\psake.4.4.1\tools\psake.psm1"

$xunit_location = ".\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe"

$configuration = "Release"

$build_file = 'default.ps1'

$projects = @(
    "Bernos.FileUploader",
	"Bernos.FileUploader.Nancy",
	"Bernos.FileUploader.StorageProviders.LocalFileSystem",
	"Bernos.FileUploader.StorageProviders.S3",
	"Bernos.FileUploader.WebApi")

$properties = @{
    "configuration"		= $configuration;
    "version"			= $version;
    "projects"			= $projects;
	"xunit_location"	= $xunit_location;
}

import-module $psake_location

invoke-psake $build_file $task_list -Properties $properties