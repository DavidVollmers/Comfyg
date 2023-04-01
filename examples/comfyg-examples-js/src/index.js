import ComfygClient from '@comfyg/client'

const client = new ComfygClient(process.env.ComfygConnectionString)

await client.configuration.addValues([{ key: 'Example', value: 'example value' }])
