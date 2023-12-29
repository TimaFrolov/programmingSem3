import React, { Component } from "react";
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';

export class Tests extends Component {
  static displayName = Tests.name;

  constructor(props) {
    super(props);
    this.state = { tests: [], loading: true };
  }

  componentDidMount() {
    this.populateData();
  }

  static renderTable(tests) {
    return (
      <table className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>ИД Теста</th>
            <th>ИД Сборки</th>
            <th>Успешно</th>
            <th>Провалено</th>
            <th>Проигнорировано</th>
          </tr>
        </thead>
        <tbody>
          {tests.map(test =>
            <tr key={test.id}>
              <td> <NavLink tag={Link} to={`/test/${test.id}`}>{test.id}</NavLink></td>
              <td> <NavLink tag={Link} to={`/assemblies/${test.assembly.id}`}>{test.assembly.id}</NavLink></td>
              <td>{test.testResults.filter(t => t.exception === null && t.reason === null).length}</td>
              <td>{test.testResults.filter(t => t.exception !== null).length}</td>
              <td>{test.testResults.filter(t => t.reason !== null).length}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Tests.renderTable(this.state.tests);

    return (
      <div>
        <h1 id="tableLabel">Прошедшие тесты</h1>
        {contents}
      </div>
    );
  }

  async populateData() {
    const response = await fetch('tests');
    const data = await response.json();
    this.setState({ tests: data, loading: false });
  }
}
