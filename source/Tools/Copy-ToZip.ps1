﻿function Copy-ToZip
{
    <#
    .Synopsis
    Compresses files and adds them to a ZIP file.
        
    .Description
    The Copy-ToZip function compresses files and
    adds the compressed files to a ZIP archive file. 

    If the ZIP file does not exist, this function
    creates it. You can also use this function to add
    compressed files to a ZIP file that you create by
    using the New-Zip function.
    
    .Parameter File             
    Enter the path (optional) and name of the file
    to compress.
              
    You can enter only one file with the File parameter.
    To submit multiple files, pipe the files to the
    Copy-ToZip function.

    This parameter is required. The default path is
    is the current directory.

    .Parameter ZipFile
    
    Enter the path (optional) and file name of the 
    ZIP file to which the files are copied. The ZIP
    file should have a .zip file name extension.
    If the specified ZIP file does not exist, Copy-ToZip
    creates it.

    This parameter is required. The default path
    is the current directory.        
    
    .Parameter HideProgress
     
    Hides the progress bar that Copy-ToZip displays by default
    
    .Parameter Force
    
    Copies read-only files to the ZIP archive file.        
      
    .Example
    copy-toZip –file Report.docx –zipfile Manager.zip

    .Example
    copy-toZip –file $home\documents\Report.docx –zipfile C:\Zip\Manager.zip -force

    .Example
    copy-toZip Report.docx Manager.zip –hideProgress

    .Example 
    dir .\*.xml | copy-toZip –zipfile XMLFiles.zip

    .Link
    New-Zip
    #>

    param(
    [Parameter(Mandatory=$true, Position=0, ValueFromPipelineByPropertyName=$true)]
    [Alias('FullName')]
    [String]$File,

    [Parameter(Mandatory=$true,Position=1)]
    [String]$ZipFile,
    
    [Switch]$HideProgress,

    [Switch]$Force
    )
    
    Begin {
        $ShellApplication = New-Object -ComObject Shell.Application
        if (-not (Test-Path $ZipFile)) {
            New-Zip $ZipFile
        }
        $Path = Resolve-Path $ZipFile
        $ZipPackage =$ShellApplication.Namespace("$Path")
    }
    Process {
        if (-not (Test-Path -literalpath $File)) { return }        
        if (-not $hideProgress) {
            $perc +=5 
            if ($perc -gt 100) { $perc = 0 } 
            Write-Progress "Copying to $ZipFile" $File -PercentComplete $perc
        }
        $Flags = 0
        if ($force) {
            $flags = 16 -bor 1024 -bor 64 -bor 512
        } 
        Write-Verbose $File
        $ZipPackage.CopyHere($File, $flags)
        Start-Sleep -Milliseconds 500
    }
}