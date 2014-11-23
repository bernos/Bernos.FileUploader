Properties {
    $version = $null
    $projects = $null
    $configuration = "Release"
	$xunit_location = $null
	$test_result_dir = ".\TestResults"
	$package_dir = ".\Dist"
}

Task Default -Depends UnitTests

Task Publish -Depends Package {
    foreach($project in $projects) {
        Get-ChildItem -Path $package_dir | Where-Object -FilterScript {
            ($_.Name.Contains("$project.$version")) -and !($_.Name.Contains(".symbols")) -and ($_.Extension -eq '.nupkg')    
        } | ForEach-Object {
            exec { nuget push $_.FullName }
        }
    }
}

Task Package -Depends Set-Versions,UnitTests {

	New-Item -Force -ItemType Directory -Path $package_dir

    foreach($project in $projects) {
        Get-ChildItem -Path "$project\*.csproj" | ForEach-Object {        			    
            exec { nuget pack -sym $_.FullName -Prop Configuration=$configuration -OutputDirectory $package_dir }
        }        
    }
}

Task Build -Depends Clean {
    Exec { msbuild "$Solution" /t:Build /p:Configuration=$configuration /p:VisualStudioVersion=12.0 /v:normal /nologo } 
}

Task Clean {
    Exec { msbuild "$Solution" /t:Clean /p:Configuration=$configuration /p:VisualStudioVersion=12.0 /v:normal /nologo } 
}

Task UnitTests -Depends Build {	

	New-Item -Force -ItemType Directory -Path $test_result_dir
	Remove-Item $test_result_dir\* -Recurse

	Get-ChildItem -Recurse -Include *Tests.csproj | ForEach-Object {
		"Running unit tests from $_.BaseName" | Write-Host -ForegroundColor Yellow

		$test_assembly = $_.DirectoryName + "\bin\" + $configuration + "\" + $_.BaseName + ".dll"
		$xml_file = $test_result_dir + "\report.xml"

		exec { .$xunit_location "$test_assembly" /nunit $xml_file } "One or more unit tests failed!"

		"" | Write-Host
	}
}

Task Set-Versions {
    if ($version) {        
        Get-ChildItem -Recurse -Force | Where-Object { $_.Name -eq "AssemblyInfo.cs" } | ForEach-Object {
            (Get-Content $_.FullName) | ForEach-Object {
                ($_ -replace 'AssemblyVersion\(.*\)', ('AssemblyVersion("' + $version + '")')) -replace 'AssemblyFileVersion\(.*\)', ('AssemblyFileVersion("' + $version + '")')
            } | Set-Content $_.FullName -Encoding UTF8
        }

        Get-ChildItem -Recurse -Force | Where-Object { $_.Name -like "*.nuspec" } | ForEach-Object {
            (Get-Content $_.FullName) | ForEach-Object {
                ($_ -replace '<dependency id="Bernos.FileUploader" version=".*"/>', ('<dependency id="Bernos.FileUploader" version="' + $version + '"/>'))
            } | Set-Content $_.FullName -Encoding UTF8
        }

    } else {
        throw "Please specify a version number."
    }    
}
