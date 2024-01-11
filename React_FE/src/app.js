import React from 'react'
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom'
import NavigationBar from './navigation-bar'
//import Home from './home/home';
import Auth from './auth/auth'
import User from './user/user-container'
import ErrorPage from './commons/errorhandling/error-page';
import styles from './commons/styles/project-style.css';
import DeviceContainer from './device/device-container';
import HourlyConsumptionGraph from './monitoringcom/components/hourly-consumption-graph';
import SocketConnection from './monitoringcom/SocketConnection'
import ChatComponent from './user/chat-component'

class App extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isLoggedIn: false,
            //webSocket: null,
        };
    }
    handleLoginSuccess = () => {
        console.log("Salut");
        this.setState({ isLoggedIn: true });
        localStorage.setItem('isLoggedIn', true);
        //this.establishWebSocketConnection();
    };
    handleLogout = () => {
        console.log("NeSalut");
        this.setState({ isLoggedIn: false });
        localStorage.removeItem('isLoggedIn');
    }

    render() {

        return (
            <div className={styles.back}>

                <Router>
                    <div>
                        {/*
                        {this.state.isLoggedIn ?
                            <>
                                <SocketConnection />
                            </>
                            : null
                        }
                    */}
                        <NavigationBar onLogoutSuccess={this.handleLogout} />
                        <Switch>

                            <Route
                                exact
                                path='/'
                                render={() => <Auth onLoginSuccess={this.handleLoginSuccess} />}
                            />
                            <Route
                                exact
                                path='/chat'
                                render={() => <ChatComponent />}
                            />
                            <Route
                                exact
                                path='/admin'
                                render={() => <User />}
                            />
                            <Route
                                path="/devices/:userId" component={DeviceContainer} />

                            <Route path="/monitoring-consumption/:deviceId" component={HourlyConsumptionGraph} />

                            {/*Error*/}
                            <Route
                                exact
                                path='/error'
                                render={() => <ErrorPage />}
                            />

                            <Route render={() => <ErrorPage />} />
                        </Switch>
                    </div>
                </Router>

            </div>
        )
    };
}

export default App
