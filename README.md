# WebCrawl MCP Server

<!-- mcp-name: io.github.vikas0sharma/WebCrawl -->

An MCP (Model Context Protocol) server that crawls web pages and returns their text content.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Tools

### `Crawl`

Crawls a web page at the given URL and returns its text content.

**Parameters:**

| Name  | Type     | Description                        |
|-------|----------|------------------------------------|
| `url` | `string` | The URL of the web page to crawl.  |

The tool fetches the HTML from the specified URL, strips scripts, styles, and tags, decodes HTML entities, and returns clean readable text.

## Getting Started

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project WebCrawl
```

The server communicates over **stdio** using the MCP protocol.

### Install as an MCP Server

Add the following to your MCP client configuration:

```json
{
  "mcpServers": {
    "WebCrawl": {
      "type": "stdio",
      "command": "dnx",
      "args": ["WebCrawl", "--version", "0.0.1", "--yes"]
    }
  }
}
```

## Project Structure

```
WebCrawl/
├── Program.cs        # Host setup and MCP server registration
├── WebCrawlTool.cs   # Crawl tool implementation
└── WebCrawl.csproj   # Project file
```

## License

This project is open source. See the repository for license details.
