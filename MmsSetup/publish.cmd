@ECHO OFF
set WEB_ROOT=C:\inetpub\MMSWebApi
set PROJECT_ROOT=C:\projects\ey\Code\Rmi\EY\Mms\WebApi
     
echo Publishing site %PROJECT_ROOT% to %WEB_ROOT% 
     
del /S /Q %WEB_ROOT%\*.* || goto Error
rmdir /S /Q %WEB_ROOT%\ || goto Error
aspnet_compiler -p "%PROJECT_ROOT%" /v /commercesite /d "%WEB_ROOT%" || goto Error 
     
goto Success
   :Error
echo Site was not published 
    
goto End
   
 :Success
    echo Site published successfully     
 :End