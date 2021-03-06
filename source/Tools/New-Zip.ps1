﻿function New-Zip {
    <#
    .Synopsis
    Creates a new zip archive.

    .Description
    The New-Zip function creates a ZIP archive file (.zip)
    with no contents (no compressed files). To add files to
    the ZIP archive, use the Copy-ToZip function.

    .Parameter PassThru
    
    Returns an object that represents the zip file. By
    default, this function does not generate any output.

    .Parameter Path
    
    Enter a path (optional) and name for the zip file that
    New-Zip creates. The file name should have a .zip file
    name extension.
        
    The file name is required. The default path is
    the current directory. 
    
    .Parameter Force
    
    Overwrites existing zip files if they exist
         
    .Example
    New-Zip Try.zip

    .Example
    New-Zip d:\ps-test\NewFiles.zip
           
    .Example
    New-Zip –path Try.zip –PassThru
     
    .Link
    Copy-ToZip
    #>
    param(
    [Parameter(Mandatory=$true,
        Position=0,
        ValueFromPipeline=$true)]
    [String]
    $Path,
    
    [Switch]
    $PassThru,
    
    [Switch]
    $Force
    )
    
    Process {
        if (Test-Path $path)
	    {
            if (-not $Force) { 
                return
            }
        }
        Set-Content $path ("PK" + [char]5 + [char]6 + ("$([char]0)" * 18))
        $item = Get-Item $path
        $item.IsReadOnly = $false	
        if ($passThru) { $item } 
    }
}
