#!/bin/sh

azurite > /dev/null &

dotnet Comfyg.Store.Api.dll
