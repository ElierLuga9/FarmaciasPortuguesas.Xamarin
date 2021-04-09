param ([string] $ProjectDir, [string] $ConfigurationName)
Write-Host "ProjectDir: $ProjectDir"
Write-Host "ConfigurationName: $ConfigurationName"

$ManifestPath = $ProjectDir + "Properties\AndroidManifest.xml"

Write-Host "ManifestPath: $ManifestPath"

[xml] $xdoc = Get-Content $ManifestPath

$package = $xdoc.manifest.package

If ($ConfigurationName.Contains("Prod") -and $package.EndsWith(".qa")) 
{ 
    $package = $package.Replace(".qa", "") 
}
If ($ConfigurationName.Contains("QA") -and -not $package.EndsWith(".qa")) 
{ 
    $package = $package + ".qa" 
}

If ($package -ne $xdoc.manifest.package) 
{
    $xdoc.manifest.package = $package
    $xdoc.Save($ManifestPath)
    Write-Host "AndroidManifest.xml package name updated to $package"
}