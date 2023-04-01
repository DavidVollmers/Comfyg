import { ComfygValue } from './comfyg-value.js'
import { Stream } from 'stream'
import { ComfygClient } from './comfyg-client.js'

export class ValueOperations {
  public constructor(private readonly _client: ComfygClient, private readonly _operation: string) {}

  public async getValues(since: Date | null = null, signal: AbortSignal | null = null): Promise<ComfygValue[]> {
    throw new Error('Not Implemented.')
  }

  public async getValuesAsync(
    since: Date | null = null,
    signal: AbortSignal | null = null,
  ): Promise<AsyncIterator<ComfygValue>> {
    throw new Error('Not Implemented.')
  }

  public async getValuesStream(since: Date | null = null, signal: AbortSignal | null = null): Promise<Stream> {
    throw new Error('Not Implemented.')
  }

  public async addValues(values: ComfygValue[], signal: AbortSignal | null = null): Promise<void> {
    if (values == null) throw new Error('Value cannot be null. Parameter name: values')
    const response = await this._client.__sendRequest('/' + this._operation, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ values }),
      signal,
    })
    if (!response.ok) {
      throw new Error(`Invalid status code when trying to add values (${this._operation}): ` + response.status)
    }
  }
}
