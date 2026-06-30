   New-Service -Name "HPTHistoryDownloader" `
               -BinaryPathName "`"C:\path\to\your\HPTHistoryDownloader.exe`"" `
               -DisplayName "HPT History Downloader Service" `
               -Description "Downloads and processes horse racing history data" `
               -StartupType Automatic
