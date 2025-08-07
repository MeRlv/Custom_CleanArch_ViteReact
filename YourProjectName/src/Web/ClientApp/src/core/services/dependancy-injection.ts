import { ContainerBuilder } from 'diod';
import { CredentialsManager } from '../../api/credentials-manager';
import { ApiClient } from '../../api/api-client';

const builder = new ContainerBuilder();

// No dependencies
builder.registerAndUse(CredentialsManager).asSingleton();

// Dependencies /!\ Order is important
builder
  .register(ApiClient)
  .useFactory(() => new ApiClient())
  .asSingleton();

const DIContainer = builder.build({ autowire: false });
export default DIContainer;
