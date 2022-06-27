# Kubernetes Proof of Concept

Notes for local kubernetes demo.

## Pre-requisites

- Kubernetes installation such as minikube
- kubectl

## Some Commands

`minikube start`

`eval $(minikube docker-env)`

`minikube dashboard`

`kubectl get pods`

`kubectl apply -f kubernetes.yml`

`kubectl delete -f kubernetes.yml`

`minikube service omega-web`

`kubectl describe pod omega-core`

`kubectl logs omega-core-58dc4cbb45-r2jz9`
