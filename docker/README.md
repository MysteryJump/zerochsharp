# Install ZerochSharp with Docker and docker-compose

## Install Docker and docker-compose

[See also](https://docs.docker.com/install/linux/docker-ce/ubuntu/)

## Install nginx

[See also](https://docs.nginx.com/nginx/admin-guide/installing-nginx/installing-nginx-open-source/)

We recommend to install nginx to local, not on docker for now.

You need to configure settings such as reverse proxy, SSL.
Below codes is a sample configuration of reverse proxy.

```Nginx
server {
        listen 80;
        server_name example.com;

        location / {

                proxy_pass      http://localhost:5000/;
        }

        proxy_set_header    Host    $host;
        proxy_set_header    X-Real-IP       $remote_addr;
        proxy_set_header    X-Forwarded-Host        $host;
        proxy_set_header    X-Forwarded-Server      $host;
        proxy_set_header    X-Forwarded-For $proxy_add_x_forwarded_for;
}
```

## Startup Zerochsharp

This section is using Ubuntu distrobution.

1. Install required packages (maybe skippable)

```bash
sudo apt update && sudo apt install git -y
```

1. Clone the zerochsharp repository

```bash
git clone https://github.com/MysteryJump/zerochsharp.git
```

1. Build and startup Zerochsharp

```bash
cd zerochsharp/docker
docker-compose build
docker-compose up -d
```
