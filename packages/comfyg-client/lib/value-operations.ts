import { ComfygValue } from './comfyg-value.js'
import { ComfygClient } from './comfyg-client.js'
import { Response } from 'node-fetch'
import ReadableStream = NodeJS.ReadableStream

export class ValueOperations {
  public constructor(private readonly _client: ComfygClient, private readonly _operation: string) {}

  public async getValues(since: Date | null = null, signal: AbortSignal | null = null): Promise<ComfygValue[]> {
    const response = await this.getValuesResponse(since, signal)
    const json = await response.json()
    return <ComfygValue[]>json
  }

  public async *getValuesAsync(
    since: Date | null = null,
    signal: AbortSignal | null = null,
  ): AsyncGenerator<ComfygValue> {
    const response = await this.getValuesResponse(since, signal)
    if (response.body == null) return
    for await (const chunk of response.body) {
      yield* JSON.parse(chunk.toString())
    }
  }

  public async getValuesStream(
    since: Date | null = null,
    signal: AbortSignal | null = null,
  ): Promise<ReadableStream | null> {
    const response = await this.getValuesResponse(since, signal)
    return response.body
  }

  private async getValuesResponse(since: Date | null = null, signal: AbortSignal | null = null): Promise<Response> {
    const response = await this._client.__sendRequest(
      '/' + this._operation,
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
}
