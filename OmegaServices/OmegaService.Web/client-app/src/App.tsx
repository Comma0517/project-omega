import './App.css'
import axios from 'axios'
import { useState } from 'react'
import Nav from './components/nav'
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link
} from 'react-router-dom'

function App() {
  return (
    <Router>
      <Nav></Nav>
      <main role="main" className="container" style={{ paddingTop: 80 }}>
        <Switch>
          <Route path="/" exact={true}>
            <Home />
          </Route>
          <Route path="*">
            <NotFound></NotFound>
          </Route>
        </Switch>
      </main>
    </Router>
  )
}

function Home() {
  const [loading, setLoading] = useState(false)
  const [randomString, setRandomString] = useState('')
  const [coreMessage, setCoreMessage] = useState('')

  async function getRandomString() {
    if (loading) {
      return
    }
    setLoading(true)
    setRandomString('')
    try {
      let response = await axios.get(`/api/Random`)
      console.log(response)
      setRandomString(response.data)
    } catch (err) {
      console.log('API error: ', err)
    } finally {
      setLoading(false)
    }
  }

  async function serviceToServiceTest() {
    if (loading) {
      return
    }
    setCoreMessage('')
    setLoading(true)
    try {
      let response = await axios.get(`/api/SomeWebEndpoint`)
      console.log(response)
      setCoreMessage(JSON.stringify(response.data))
    } catch (err) {
      console.log('API error: ', err)
    } finally {
      setLoading(false)
    }
  }

  return (
    <>
      <div className="jumbotron">
        <h1 className="display-3">Project Omega</h1>
        <p className="lead">The last enterprise web architecture pattern you'll ever need. Until the next one...</p>
        {/* <hr className="my-4" /> */}
        {/* <p>It uses utility classes for typography and spacing to space content out within the larger container.</p>
          <p className="lead">
            <a className="btn btn-primary btn-lg" href="#" role="button">Learn more</a>
          </p> */}
      </div>
      <h3>Service Call Demo</h3>
      <p><button className="btn btn-primary" onClick={() => getRandomString()}>Get a Random String</button>&nbsp;&nbsp;<span>{randomString}</span></p>
      <p><button className="btn btn-primary" onClick={() => serviceToServiceTest()}>Service to Service Test</button>&nbsp;&nbsp;<span>{coreMessage}</span></p>
    </>
  )
}

function NotFound() {
  return (
    <div>
      <h1>Not Found</h1>
    </div>
  )
}

export default App
