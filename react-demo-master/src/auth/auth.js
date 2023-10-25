import React from 'react';
import { FormGroup, Input, Label, Button } from 'reactstrap';
import { register, login } from '../user/api/auth-api'; 

class Auth extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };

        this.handleLogin = this.handleLogin.bind(this);
        this.handleRegister = this.handleRegister.bind(this);
    }

    handleLogin(event) {
        event.preventDefault();

        const user = {
            Username: this.state.username,
            Password: this.state.password
        };

        login(user, (response) => {
            if (response.ok) {
                
                if (parseInt(localStorage.getItem('role'), 10) === 1) { // admin
                    window.location.href = "/admin"; 
                } else {
                    window.location.href = "/user"; 
                }
            } else {
                console.error("Failed to login");
            }
        });
    }

    handleRegister(event) {
        event.preventDefault();

        const user = {
            Username: this.state.username,
            Password: this.state.password
        };

        register(user, (response) => {
            if (response.ok) {
                console.log("Successfully registered. You can now login.");
            } else {
                console.error("Failed to register");
            }
        });
    }

    render() {
        return (
            <div>
                <h2>Authentication</h2>

                <FormGroup>
                    <Label for='usernameField'> Username: </Label>
                    <Input type='text' name='username' id='usernameField'
                        value={this.state.username}
                        onChange={e => this.setState({ username: e.target.value })}
                        required
                    />
                </FormGroup>

                <FormGroup>
                    <Label for='passwordField'> Password: </Label>
                    <Input type='password' name='password' id='passwordField'
                        value={this.state.password}
                        onChange={e => this.setState({ password: e.target.value })}
                        required
                    />
                </FormGroup>

                <Button onClick={this.handleLogin}>Login</Button>
                <Button onClick={this.handleRegister}>Register</Button>
            </div>
        );
    }
}

export default Auth;
