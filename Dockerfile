FROM microsoft/dotnet:onbuild
MAINTAINER joncloud <jdberube@gmail.com>

WORKDIR /dotnetapp/tests/ThorNet.UnitTests/
ENTRYPOINT dotnet test
