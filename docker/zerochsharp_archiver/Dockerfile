FROM mcr.microsoft.com/dotnet/core/sdk:3.1

LABEL maintainer="MysteryJump <news7vip@gmail.com>"

# Preparation for auto archiver
WORKDIR /
RUN git clone https://github.com/MysteryJump/zerochsharp

#WORKDIR /zerochsharp/ZerochSharp
#RUN sed -i -e 's/localhost/zerochsharp_mariadb/' appsettings.json

WORKDIR /zerochsharp/src/ZerochSharp.AutoArchiver
RUN sed -i -e 's/localhost/zerochsharp_mariadb/' Program.cs
RUN echo "#!/bin/sh\nsleep 180; dotnet run --configuration Release" > startup.sh
RUN chmod +x startup.sh

ENTRYPOINT [ "./startup.sh" ]
