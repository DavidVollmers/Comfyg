import { ComfygValue } from './comfyg-value.js'
import { ComfygClient } from './comfyg-client.js'
import { Response } from 'node-fetch'
import ReadableStream = NodeJS.ReadableStream
import { Permissions } from './permissions.js'
import ComfygConstants from './comfyg-constants.js'

export class ValueOperations {
  public constructor(private readonly _client: ComfygClient, private readonly _operation: string) {}

  public async getValues(
    since: Date | null = null,
    tags: string[] = [],
    signal: AbortSignal | null = null,
  ): Promise<ComfygValue[]> {
    const response = await this.getValuesResponse(since, tags, signal)
    const json = await response.json()
    return <ComfygValue[]>json
  }

  public async *getValuesAsync(
    since: Date | null = null,
    tags: string[] = [],
    signal: AbortSignal | null = null,
  ): AsyncGenerator<ComfygValue> {
    const response = await this.getValuesResponse(since, tags, signal)
    if (response.body == null) return
    for await (const chunk of response.body) {
      yield* JSON.parse(chunk.toString())
    }
  }

  public async getValuesStream(
    since: Date | null = null,
    tags: string[] = [],
    signal: AbortSignal | null = null,
  ): Promise<ReadableStream | null> {
    const response = await this.getValuesResponse(since, tags, signal)
    return response.body
  }

  private async getValuesResponse(
    since: Date | null = null,
    tags: string[] = [],
    signal: AbortSignal | null = null,
  ): Promise<Response> {
    let uri = '/' + this._operation
    const params = new URLSearchParams()
    if (since) {
      params.set('since', since.toISOString())
    }
    if (tags && tags.length) {
      for (const tag of tags) {
        params.append('tags', encodeURIComponent(tag))
      }
    }
    if ([...params].length) uri += '?' + params.toString()
    const response = await this._client.__sendRequest(
      uri,
      {
        method: 'GET',
      },
      signal,
    )
    if (!response.ok) {
      throw new Error(`Invalid status code when trying to get values (${this._operation}): ` + response.status)
    }
    return response
  }

  public async getValue(
    key: string,
    version: string | null = null,
    signal: AbortSignal | null = null,
  ): Promise<ComfygValue> {
    if (key == null) throw new Error('Value cannot be null. Parameter name: key')
    let uri = '/' + this._operation + '/' + encodeURIComponent(key)
    if (version) uri += '/' + encodeURIComponent(version)
    const response = await this._client.__sendRequest(
      uri,
      {
        method: 'GET',
      },
      signal,
    )
    if (!response.ok) {
      throw new Error(`Invalid status code when trying to get value (${this._operation}): ` + response.status)
    }
    const json = await response.json()
    return <ComfygValue>json
  }

  public async addValues(values: ComfygValue[], signal: AbortSignal | null = null): Promise<void> {
    if (values == null) throw new Error('Value cannot be null. Parameter name: values')
    const response = await this._client.__sendRequest(
      '/' + this._operation,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ values }),
      },
      signal,
    )
    if (!response.ok) {
      throw new Error(`Invalid status code when trying to add values (${this._operation}): ` + response.status)
    }
  }

  public async tagValue(
    key: string,
    tag: string,
    version: string = ComfygConstants.LatestVersion,
    signal: AbortSignal | null = null,
  ): Promise<ComfygValue> {
    if (key == null) throw new Error('Value cannot be null. Parameter name: key')
    if (tag == null) throw new Error('Value cannot be null. Parameter name: tag')
    if (version == null) throw new Error('Value cannot be null. Parameter name: version')
    const response = await this._client.__sendRequest(
      '/' + this._operation + '/tags',
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ key, tag, version }),
      },
      signal,
    )
    if (!response.ok) {
      throw new Error(`Invalid status code when trying to tag value (${this._operation}): ` + response.status)
    }
    const json = await response.json()
    return <ComfygValue>json
  }

  public async setPermission(
    clientId: string,
    key: string,
    permissions: Permissions,
    signal: AbortSignal | null = null,
  ): Promise<void> {
    if (clientId == null) throw new Error('Value cannot be null. Parameter name: clientId')
    if (key == null) throw new Error('Value cannot be null. Parameter name: key')
    if (permissions == null) throw new Error('Value cannot be null. Parameter name: permissions')
    const response = await this._client.__sendRequest(
      '/permissions/' + this._operation,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify([{ clientId, key, permissions }]),
      },
      signal,
    )
    if (!response.ok) {
      throw new Error(`Invalid status code when trying set permission (${this._operation}): ` + response.status)
    }
  }
}
