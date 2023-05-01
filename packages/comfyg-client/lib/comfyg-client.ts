import { ConnectionResponse } from './responses/connection-response.js'
import fetch, { FormData, RequestInit, Response } from 'node-fetch'
import jwt from 'jwt-simple'
import { Client } from './client.js'
import { SetupClientResponse } from './responses/setup-client-response.js'
import { ValueOperations } from './value-operations.js'
import { Permissions } from './permissions.js'

export class ComfygClient {
  private readonly _endpoint: string
  private readonly _clientId: string
  private readonly _clientSecret: Buffer
  private readonly _configuration: ValueOperations
  private readonly _secrets: ValueOperations
  private readonly _settings: ValueOperations
  private readonly _token: {
    raw?: string
    validTo?: Date
  } = {}

  public get configuration(): ValueOperations {
    return this._configuration
  }

  public get secrets(): ValueOperations {
    return this._secrets
  }

  public get settings(): ValueOperations {
    return this._settings
  }

  //TODO support asymmetric clients
  public constructor(connectionString: string) {
    if (connectionString == null) throw new Error('Value cannot be null. Parameter name: connectionString')
    try {
      const connectionInformation = connectionString
        .split(';')
        .map((i) => i.split('='))
        .filter((i) => i.length >= 2)
        .map((i) => {
          return {
            key: i[0],
            value: i.slice(1).join('='),
          }
        })
      const endpoint = connectionInformation.filter((i) => i.key === 'Endpoint')[0]
      if (!endpoint) throw new Error('Missing "Endpoint" information.')
      this._endpoint = endpoint.value
      const clientId = connectionInformation.filter((i) => i.key === 'ClientId')[0]
      if (!clientId) throw new Error('Missing "ClientId" information.')
      this._clientId = clientId.value
      const clientSecret = connectionInformation.filter((i) => i.key === 'ClientSecret')[0]
      if (!clientSecret) throw new Error('Missing "ClientSecret" information.')
      this._clientSecret = Buffer.from(clientSecret.value, 'base64')
      if (this._clientSecret.length < 16) throw new Error('Client secret must be at least 16 bytes long.')
    } catch (e: any) {
      throw new Error('Invalid connection string: ' + e.message)
    }
    this._configuration = new ValueOperations(this, 'configuration')
    this._secrets = new ValueOperations(this, 'secrets')
    this._settings = new ValueOperations(this, 'settings')
  }

  public async establishConnection(signal: AbortSignal | null = null): Promise<ConnectionResponse> {
    const response = await this.__sendRequest(
      '/connections/establish',
      {
        method: 'POST',
      },
      signal,
    )
    if (!response.ok) {
      throw new Error('Invalid status code when trying to establish connection: ' + response.status)
    }
    const json = await response.json()
    return <ConnectionResponse>json
  }

  //TODO support asymmetric clients
  public async setupClient(client: Client, signal: AbortSignal | null = null): Promise<SetupClientResponse> {
    if (client == null) throw new Error('Value cannot be null. Parameter name: client')
    const formData = new FormData()
    formData.set('ClientId', client.clientId)
    formData.set('FriendlyName', client.friendlyName)
    const response = await this.__sendRequest(
      '/setup/client',
      {
        method: 'POST',
        body: formData,
      },
      signal,
    )
    if (!response.ok) {
      throw new Error('Invalid status code when trying to setup client: ' + response.status)
    }
    const json = await response.json()
    return <SetupClientResponse>json
  }

  public async setPermissions(
    clientId: string,
    permissions: Permissions,
    signal: AbortSignal | null = null,
  ): Promise<void> {
    if (clientId == null) throw new Error('Value cannot be null. Parameter name: clientId')
    if (permissions == null) throw new Error('Value cannot be null. Parameter name: permissions')
    const response = await this.__sendRequest(
      '/permissions',
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ clientId, permissions }),
      },
      signal,
    )
    if (!response.ok) {
      throw new Error(`Invalid status code when trying set permissions: ` + response.status)
    }
  }

  public async __sendRequest(
    relativeUrl: string,
    init: RequestInit,
    signal: AbortSignal | null = null,
  ): Promise<Response> {
    const token = this.createToken()
    return await fetch(this._endpoint + relativeUrl, {
      ...init,
      headers: {
        ...init.headers,
        Authorization: 'Bearer ' + token,
      },
      signal,
    })
  }

  private createToken(): string {
    const nowDate = new Date()
    nowDate.setMinutes(nowDate.getMinutes() + 5)
    if (this._token.raw && this._token.validTo! > nowDate) return this._token.raw
    const expDate = new Date()
    expDate.setDate(expDate.getDate() + 1)
    expDate.setMinutes(expDate.getMinutes() + 5)
    this._token.validTo = expDate
    return (this._token.raw = jwt.encode(
      {
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': this._clientId,
        iss: this._clientId,
        aud: this._clientId,
        exp: Math.floor(expDate.valueOf() / 1000),
      },
      // https://github.com/hokaccha/node-jwt-simple/pull/99
      <any>this._clientSecret,
      'HS512',
    ))
  }
}
