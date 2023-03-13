FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY bin/Release/net6.0/publish/ .

ENTRYPOINT ["dotnet", "Comfyg.Api.dll"]