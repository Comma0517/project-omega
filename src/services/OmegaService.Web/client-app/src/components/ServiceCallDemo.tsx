import {useState} from 'react';
import axios from 'axios';

interface ServiceCallDemoProps {
  buttonLabel: string
  serviceUrl: string
}

export function ServiceCallDemo({buttonLabel, serviceUrl}: ServiceCallDemoProps) {
  const [isLoading, setIsLoading] = useState(false)
  const [result, setResult] = useState('')
  
  async function doServiceCall() {
    if (isLoading) {
      return
    }
    setIsLoading(true)
    setResult('loading...')
    try {
      let response = await axios.get(serviceUrl)
      console.log(response)
      setResult(JSON.stringify(response.data))
    } catch (err) {
      console.log('API error: ', err)
      setResult('error (see console)')
    } finally {
      setIsLoading(false)
    }
  }
  
  return (
    <div className="container pb-3">
      <div className="row">
        <div className="col-md-3 pb-3 pl-0">
          <button className="btn btn-primary" onClick={() => doServiceCall()}>{buttonLabel}</button>
        </div>
        <div className="col-md-9 border rounded border-info">
          <div style={{height: '170px', overflowWrap: 'anywhere', overflow: 'auto'}}>{result}</div>
        </div>
      </div>
    </div>
  )
}
