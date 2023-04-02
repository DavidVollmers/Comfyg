import ComfygClient from '@comfyg/client'

const client = new ComfygClient(process.env.ComfygConnectionString)

const values = await client.configuration.getValuesAsync()
for await (const value of values) {
  console.log(value)
}
