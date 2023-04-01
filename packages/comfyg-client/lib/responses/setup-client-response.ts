import { Client } from '../client.js'

export interface SetupClientResponse {
  readonly client: Client
  readonly clientSecret: string
}
