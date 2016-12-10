# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('pools', '0002_auto_20150428_1413'),
    ]

    operations = [
        migrations.RenameModel(
            old_name='API',
            new_name='STH',
        ),
    ]
