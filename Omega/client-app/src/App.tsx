import './App.css'
import axios from 'axios'
import { useState } from 'react'
import Nav from './components/nav'

function App() {
  const [randomString, setRandomString] = useState('')
  const [coreMessage, setCoreMessage] = useState('')

  async function getRandomString() {
    setRandomString('')
    try {
      let response = await axios.get(`/api/Random`)
      console.log(response)
      setRandomString(response.data)
    } catch (err) {
      console.log('API error: ', err)
    }
  }

  async function serviceToServiceTest() {
    setCoreMessage('')
    try {
      let response = await axios.get(`/api/SomeWeb`)
      console.log(response)
      setCoreMessage(JSON.stringify(response.data))
    } catch (err) {
      console.log('API error: ', err)
    }
  }

  return (
    <>
      <Nav></Nav>
      <main role="main" className="container" style={{ paddingTop: 80 }}>
        <h1>Project Omega</h1>
        <p className="lead">The last enterprise web architecture pattern you'll ever need.<br /> Until the next one...</p>
        <h3>Service Call Demo</h3>
        <p><button className="btn btn-primary" onClick={() => getRandomString()}>Get a Random String</button>&nbsp;&nbsp;<span>{randomString}</span></p>
        <p><button className="btn btn-primary" onClick={() => serviceToServiceTest()}>Service to Service Test</button>&nbsp;&nbsp;<span>{coreMessage}</span></p>
      </main>
    </>
  );
}

export default App;
