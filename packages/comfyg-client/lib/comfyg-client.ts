export default class ComfygClient {
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
    } catch (e) {
      throw new Error('Invalid connection string: ' + e.message)
    }
  }
}
