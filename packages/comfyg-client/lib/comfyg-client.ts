import { ConnectionResponse } from './responses/connection-response.js'
import fetch from 'node-fetch'
import * as jwt from 'jwt-simple'

export class ComfygClient {
  private readonly _endpoint: string
  private readonly _clientId: string
  private readonly _clientSecret: string
  private readonly _token: {
    raw?: string
    validTo?: Date
  } = {}

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
      this._clientSecret = clientSecret.value
    } catch (e: any) {
      throw new Error('Invalid connection string: ' + e.message)
    }
  }

  public async establishConnection(): Promise<ConnectionResponse> {
    const token = this.createToken()
    const response = await fetch(this._endpoint, {
      method: 'POST',
      headers: {
        Authorization: 'Bearer ' + token,
      },
    })
    if (response.status < 200 || response.status > 299) {
      throw new Error('Invalid status code when trying to establish connection: ' + response.status)
    }
    const json = await response.json()
    return <ConnectionResponse>json
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
      this._clientSecret,
      'HS512',
    ))
  }
}
