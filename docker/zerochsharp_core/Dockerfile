FROM mcr.microsoft.com/dotnet/core/sdk:3.1

LABEL maintainer="MysteryJump <news7vip@gmail.com>"

# Preparation for run application
WORKDIR /
RUN mkdir zerochsharp_core
WORKDIR /zerochsharp_core
RUN wget https://s3-ap-northeast-1.amazonaws.com/zerochsharp.com/build/nightly_linux-x64.tar.gz
RUN tar -xvzf nightly_linux-x64.tar.gz
RUN rm nightly_linux-x64.tar.gz

WORKDIR /zerochsharp_core/src/nightly_published
RUN sed -i -e 's/localhost/zerochsharp_mariadb/' appsettings.json
RUN chmod +x ZerochSharp
RUN echo "#!/bin/sh\nsleep 150; ./ZerochSharp" > startup.sh
RUN chmod +x startup.sh
RUN rm -r plugins

EXPOSE 5000
ENTRYPOINT ["./startup.sh"]
