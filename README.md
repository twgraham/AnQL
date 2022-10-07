<br />
<p align="center">
  <a href="https://github.com/twgraham/AnQL">
    <img src="assets/images/ankles.jpg" alt="Cartoon Ankles" width="200">
  </a>

  <h3 align="center">AnQL</h3>

  <p align="center">
    Another Query Language - simple, no nonsense querying for C#/ASP.NET/TypeScript 
    <br />
    <a href="https://codecov.io/github/twgraham/AnQL">
        <img src="https://github.com/twgraham/AnQL/actions/workflows/build-test.yaml/badge.svg" />
    </a>
    <a href="https://codecov.io/github/twgraham/AnQL" > 
        <img src="https://codecov.io/github/twgraham/AnQL/branch/master/graph/badge.svg?token=FUJXCID1YL"/> 
    </a>
</p>

This project is still a work in progress.

A simple query language that can be consumed by a range of targets:

- In memory collections (C# and Typescript)
- Database ORMs (using C# Expressions)
- MongoDB

Example of the query language

```
(first_name: John or first_name: Jane) and age: > 30 and start_date: last year
```
