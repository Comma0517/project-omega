const { createProxyMiddleware } = require('http-proxy-middleware')

module.exports = function(app) {
  app.use(
    '/api',
    createProxyMiddleware({
      target: 'https://localhost:5001',
      changeOrigin: true,
      secure: false
    })
  )
  app.use(
    '/proxy',
    createProxyMiddleware({
      target: 'https://localhost:5001',
      changeOrigin: true,
      secure: false
    })
  )
}
