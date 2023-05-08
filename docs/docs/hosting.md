# Hosting the Comfyg Store

After trying out the Comfyg store on your local machine using the `comfyg setup localhost` (Read more about
it [here](cli/command_setup_localhost.md)) command you will eventually come to the point where you want to host an
instance of the Comfyg store for your staging/production environment.

In the following you will find a step by step guide on how to host your own Comfyg store.

## Docker

Although you can theoretically host the Comfyg store without Docker we strongly recommend you to use
the [official docker image](https://hub.docker.com/r/dvol/comfyg).

You can easily pull it using the [Docker CLI](https://docs.docker.com/engine/reference/commandline/cli/):

```shell
docker pull dvol/comfyg
```
