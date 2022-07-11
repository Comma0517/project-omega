import {ServiceCallDemo} from './ServiceCallDemo';

export function Home() {
  return (
    <>
      <div className="jumbotron">
        <h1 className="display-3">Project Omega</h1>
        <p className="lead">The last enterprise web architecture pattern you'll ever need. Until the next one...</p>
      </div>
      <h3>Service Call Demo</h3>
      <div><ServiceCallDemo buttonLabel='Direct Web Endpoint Test' serviceUrl='/api/Random'/></div>
      <div><ServiceCallDemo buttonLabel='Service to Service Test' serviceUrl='/api/SomeWebEndpoint'/></div>
      <div><ServiceCallDemo buttonLabel='DB Test' serviceUrl='/api/SomeWebEndpoint/OmegaUsers'/></div>
      <div><ServiceCallDemo buttonLabel='Proxy Test' serviceUrl='/api/Weather/FakeWeather'/></div>
    </>
  )
}
