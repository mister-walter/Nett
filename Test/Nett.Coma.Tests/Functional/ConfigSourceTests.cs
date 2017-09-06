﻿using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Nett.Coma.Tests.TestData;
using Nett.Tests.Util;

namespace Nett.Coma.Tests.Functional
{
    [ExcludeFromCodeCoverage]
    public sealed class ConfigSourceTests
    {
        private const string FuncGetSettingSource = "Get Setting Source";
        private const string FuncSetSettingWithSource = "Set Setting with source";

        [FFact(FuncSetSettingWithSource, "When explicit source is specified, that setting gets saved into that source")]
        public void SetSource_WhenExplitSourceSpecified_SettingGetsSavedToThatSource()
        {
            using (var scenario = GitScenario.Setup(nameof(SetSource_WhenExplitSourceSpecified_SettingGetsSavedToThatSource)))
            {
                // Arrange
                var config = scenario.CreateMergedFromDefaults();
                var oldVal = config.Get(c => c.Core.Symlinks);
                var newVal = !oldVal;

                // Act
                config.Set(c => c.Core.Symlinks, newVal, scenario.RepoSource);

                // Assert
                var repoContent = Toml.ReadFile<GitScenario.GitConfig>(scenario.RepoFile);
                repoContent.Core.Symlinks.Should().Be(newVal, "because as scope repo file was specified the value in that file needs to change");

                var sysContent = Toml.ReadFile<GitScenario.GitConfig>(scenario.SystemFile);
                sysContent.Core.Symlinks.Should().Be(oldVal, "because the setting in the not used scope should not change");
            }
        }

        [FFact(FuncGetSettingSource, "Of default initialized Git config returns the correct source")]
        public void GetSettingSource_ReturnsCorrectSource()
        {
            using (var scenario = GitScenario.Setup(nameof(GetSettingSource_ReturnsCorrectSource)))
            {
                // Arrange
                var config = scenario.CreateMergedFromDefaults();

                // Act
                var s0 = config.GetSource(c => c.Core.Symlinks);
                var s1 = config.GetSource(c => c.User.EMail);
                var s2 = config.GetSource(c => c.Core.IgnoreCase);

                // Assert
                s0.Should().BeSameAs(scenario.SystemSource);
                s1.Should().BeSameAs(scenario.UserSource);
                s2.Should().BeSameAs(scenario.RepoSource);
            }
        }
    }
}
