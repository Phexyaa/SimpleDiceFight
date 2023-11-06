#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DiceyFighty.csproj", "."]
RUN dotnet restore "./DiceyFighty.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DiceyFighty.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiceyFighty.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "DiceyFighty.dll"]



FROM ubuntu:latest
RUN apt update && apt install  openssh-server sudo -y
RUN useradd -rm -d /home/ubuntu -s /bin/bash -g root -G sudo -u 1000 admin 
RUN  echo 'admin:test'  | chpasswd
RUN sudo groupadd public
RUN useradd -rm -d /home/ubuntu -s /bin/bash -g public -u 1001 friends
RUN  echo 'friends:people'  | chpasswd
RUN service ssh start
RUN ls ~/.ssh/id_rsa
RUN ssh-keygen -l -v -N 'test'
EXPOSE 22
CMD ["/usr/sbin/sshd","-D"]