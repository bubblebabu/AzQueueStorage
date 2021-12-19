az login
az group create --name "bvbgrp" --location "centralus"
az storage account create \
  --name "bvbStorageAccount" \
  --resource-group "bvbgrp" \
  --location "centralus" \
  --sku Standard_RAGRS \
  --kind StorageV2
